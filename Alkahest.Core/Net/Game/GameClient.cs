using Alkahest.Core.Cryptography;
using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Alkahest.Core.Net.Game
{
    public sealed class GameClient
    {
        static readonly byte[] _magic = new byte[] { 0x01, 0x00, 0x00, 0x00 };

        static readonly Log _log = new Log(typeof(GameClient));

        public GameProxy Proxy { get; }

        public IPEndPoint EndPoint { get; }

        readonly Socket _clientSocket;

        GameEncryptionSession _clientEncryption;

        readonly Memory<byte> _clientSendBuffer = new byte[PacketHeader.MaxPacketSize];

        readonly GameBinaryWriter _clientSendWriter;

        readonly Socket _serverSocket;

        GameEncryptionSession _serverEncryption;

        readonly Memory<byte> _serverSendBuffer = new byte[PacketHeader.MaxPacketSize];

        readonly GameBinaryWriter _serverSendWriter;

        readonly object _encryptionLock = new object();

        volatile bool _disconnected;

        internal GameClient(GameProxy proxy, Socket socket)
        {
            Proxy = proxy;
            EndPoint = (IPEndPoint)socket.RemoteEndPoint;
            _clientSocket = socket;
            _serverSocket = new Socket(proxy.Info.RealEndPoint.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            void SetOptions(Socket socket)
            {
                socket.NoDelay = true;
                socket.ReceiveTimeout = socket.SendTimeout = (int)proxy.Timeout.TotalMilliseconds;
            }

            SetOptions(_clientSocket);
            SetOptions(_serverSocket);

            _clientSendWriter = new GameBinaryWriter(_clientSendBuffer.GetArray());
            _serverSendWriter = new GameBinaryWriter(_serverSendBuffer.GetArray());

            var args = proxy.ArgsPool.Get();

            args.Completed += OnConnect;
            args.RemoteEndPoint = Proxy.Info.RealEndPoint;

            proxy.AddClient(this);

            Connect(args);
        }

        public void Disconnect()
        {
            DisconnectInternal();

            _log.Info("Disconnected client {0} from {1} ({2})", EndPoint, Proxy.Info.Name,
                Proxy.Info.RealEndPoint);
        }

        void DisconnectInternal()
        {
            Proxy.RemoveClient(this);

            _disconnected = true;

            _serverSocket.SafeClose();
            _clientSocket.SafeClose();

            lock (_encryptionLock)
            {
                _clientEncryption.Dispose();
                _clientEncryption = null;

                _serverEncryption.Dispose();
                _serverEncryption = null;
            }

            Proxy.InvokeDisconnected(this);
        }

        static PacketHeader ReadHeader(GameBinaryReader reader)
        {
            reader.Position = 0;

            var length = (ushort)(reader.ReadUInt16() - PacketHeader.HeaderSize);
            var code = reader.ReadUInt16();

            return new PacketHeader(length, code);
        }

        static void WriteHeader(GameBinaryWriter writer, PacketHeader header)
        {
            writer.Position = 0;

            writer.WriteUInt16((ushort)(header.Length + PacketHeader.HeaderSize));
            writer.WriteUInt16(header.Code);
        }

        public bool SendToClient(RawPacket packet)
        {
            return SendToClient(packet, false);
        }

        internal bool SendToClient(RawPacket packet, bool direct)
        {
            return SendRawPacketInternal(Direction.ServerToClient, packet, _clientSendBuffer,
                _clientSendWriter, _clientSocket, _clientEncryption, false, direct);
        }

        public bool SendToServer(RawPacket packet)
        {
            return SendToServer(packet, false);
        }

        internal bool SendToServer(RawPacket packet, bool direct)
        {
            return SendRawPacketInternal(Direction.ClientToServer, packet, _serverSendBuffer,
                _serverSendWriter, _serverSocket, _serverEncryption, true, direct);
        }

        bool SendRawPacketInternal(Direction direction, RawPacket packet, Memory<byte> buffer,
            GameBinaryWriter writer, Socket socket, GameEncryptionSession encryption, bool server,
            bool direct)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (packet.Name == null)
                throw new ArgumentException("Packet has no name.", nameof(packet));

            if (packet.Payload.Length > PacketHeader.MaxPayloadSize)
                throw new ArgumentException("Packet is too large.", nameof(packet));

            if (!Proxy.Serializer.GameMessages.NameToCode.TryGetValue(packet.Name, out var code))
                throw new UnmappedMessageException();

            var payload = packet.Payload;
            var original = payload;
            var send = Proxy.InvokeSent(this, direction, code, ref payload, direct);

            if (!send)
                return false;

            // A handler might have created a packet that's too large.
            if (payload.Length > PacketHeader.MaxPayloadSize)
            {
                _log.Error(
                    "{0}: Forged packet {1} is too large ({2} bytes) to be sent correctly after running handlers; sending original",
                    direction.ToDirectionString(), packet.Name, payload.Length);

                payload = original;
            }

            lock (socket)
            {
                var header = new PacketHeader((ushort)payload.Length, code);

                WriteHeader(writer, header);
                payload.CopyTo(buffer.Slice(PacketHeader.HeaderSize, header.Length));

                try
                {
                    return SendInternal(buffer.Slice(0, header.FullLength), false, socket, encryption, server);
                }
                catch (SocketDisconnectedException)
                {
                    // Normal disconnection.
                    return false;
                }
            }
        }

        public bool SendToClient(SerializablePacket packet)
        {
            return SendToClient(packet, false);
        }

        internal bool SendToClient(SerializablePacket packet, bool direct)
        {
            return SendPacketInternal(Direction.ServerToClient, packet, _clientSendBuffer,
                _clientSendWriter, _clientSocket, _clientEncryption, false, direct);
        }

        public bool SendToServer(SerializablePacket packet)
        {
            return SendToServer(packet, false);
        }

        internal bool SendToServer(SerializablePacket packet, bool direct)
        {
            return SendPacketInternal(Direction.ClientToServer, packet, _serverSendBuffer,
                _serverSendWriter, _serverSocket, _serverEncryption, true, direct);
        }

        bool SendPacketInternal(Direction direction, SerializablePacket packet, Memory<byte> buffer,
            GameBinaryWriter writer, Socket socket, GameEncryptionSession encryption, bool server,
            bool direct)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            var payload = Proxy.Serializer.Serialize(packet).AsMemory();

            if (payload.Length > PacketHeader.MaxPayloadSize)
                throw new ArgumentException("Packet is too large.", nameof(packet));

            var code = Proxy.Serializer.GameMessages.NameToCode[packet.Name];
            var original = payload;
            var send = Proxy.InvokeSent(this, direction, code, ref payload, direct);

            if (!send)
                return false;

            // A handler might have created a packet that's too large.
            if (payload.Length > PacketHeader.MaxPayloadSize)
            {
                _log.Error(
                    "{0}: Forged packet {1} is too large ({2} bytes) to be sent correctly after running handlers; sending original",
                    direction.ToDirectionString(), packet.Name, payload.Length);

                payload = original;
            }

            lock (socket)
            {
                var header = new PacketHeader((ushort)payload.Length, code);

                WriteHeader(writer, header);
                payload.CopyTo(buffer.Slice(PacketHeader.HeaderSize, header.Length));

                try
                {
                    return SendInternal(buffer.Slice(0, header.FullLength), false, socket, encryption, server);
                }
                catch (SocketDisconnectedException)
                {
                    // Normal disconnection.
                    return false;
                }
            }
        }

        bool SendToClientInternal(Memory<byte> data, bool rethrow)
        {
            return SendInternal(data, rethrow, _clientSocket, _clientEncryption, false);
        }

        bool SendToServerInternal(Memory<byte> data, bool rethrow)
        {
            return SendInternal(data, rethrow, _serverSocket, _serverEncryption, true);
        }

        bool SendInternal(Memory<byte> data, bool rethrow, Socket socket,
            GameEncryptionSession encryption, bool server)
        {
            return RunGuarded(rethrow, server, () =>
            {
                lock (_encryptionLock)
                    if (encryption != null)
                        encryption.Encrypt(data);

                lock (socket)
                    socket.SendFull(data.GetArray());
            });
        }

        byte[] ReceiveFromClientInternal(int length)
        {
            var data = new byte[length];

            ReceiveInternal(data, _clientSocket, _clientEncryption, false);

            return data;
        }

        byte[] ReceiveFromServerInternal(int length)
        {
            var data = new byte[length];

            ReceiveInternal(data, _serverSocket, _serverEncryption, true);

            return data;
        }

        void ReceiveInternal(Memory<byte> data, Socket socket, GameEncryptionSession encryption,
            bool server)
        {
            RunGuarded(true, server, () =>
            {
                socket.ReceiveFull(data.GetArray());

                lock (_encryptionLock)
                    if (encryption != null)
                        encryption.Decrypt(data);
            });
        }

        void Connect(SocketAsyncEventArgs args)
        {
            Task.Factory.StartNew(() =>
            {
                RunGuarded(false, true, () => _serverSocket.ConnectAsync(args), async =>
                {
                    if (!async)
                        OnConnect(this, args);
                });
            }, TaskCreationOptions.LongRunning);
        }

        void OnConnect(object sender, SocketAsyncEventArgs args)
        {
            var error = args.SocketError;

            args.Completed -= OnConnect;
            Proxy.ArgsPool.TryPut(args);

            if (error != SocketError.Success && error != SocketError.OperationAborted)
            {
                DisconnectInternal();
                _log.Error("Could not connect to {0} ({1}) for client {2}: {3}", Proxy.Info.Name,
                    Proxy.Info.RealEndPoint, EndPoint, error);
                return;
            }

            _log.Info("Connected to {0} ({1}) for client {2}", Proxy.Info.Name,
                Proxy.Info.RealEndPoint, EndPoint);

            byte[] ckey1;
            byte[] ckey2;
            byte[] skey1;
            byte[] skey2;

            try
            {
                var magic = ReceiveFromServerInternal(_magic.Length);
                SendToClientInternal(magic, true);

                if (!magic.SequenceEqual(_magic))
                {
                    DisconnectInternal();
                    _log.Error("Disconnected client {0} from {1} due to incorrect magic bytes: {2}",
                        EndPoint, Proxy.Info.Name, magic.Aggregate("0x", (acc, x) => acc + x.ToString("X2")));
                    return;
                }

                ckey1 = ReceiveFromClientInternal(GameEncryptionSession.KeySize);
                SendToServerInternal(ckey1, true);

                skey1 = ReceiveFromServerInternal(GameEncryptionSession.KeySize);
                SendToClientInternal(skey1, true);

                ckey2 = ReceiveFromClientInternal(GameEncryptionSession.KeySize);
                SendToServerInternal(ckey2, true);

                skey2 = ReceiveFromServerInternal(GameEncryptionSession.KeySize);
                SendToClientInternal(skey2, true);
            }
            catch (SocketDisconnectedException)
            {
                // Normal disconnection.
                Disconnect();
                return;
            }
            catch (Exception e) when (IsSocketException(e))
            {
                // The client is already disconnected.
                return;
            }

            _clientEncryption = new GameEncryptionSession(Direction.ClientToServer, ckey1, ckey2, skey1, skey2);
            _serverEncryption = new GameEncryptionSession(Direction.ServerToClient, ckey1, ckey2, skey1, skey2);

            _log.Info("Established encrypted session for client {0}", EndPoint);

            Receive(Direction.ClientToServer, null, null, null, null, false);
            Receive(Direction.ServerToClient, null, null, null, null, false);

            Proxy.InvokeConnected(this);
        }

        void Receive(Direction direction, Memory<byte> headerBuffer, GameBinaryReader headerReader,
            GameBinaryWriter headerWriter, Memory<byte> payloadBuffer, bool nested)
        {
            if (headerBuffer.IsEmpty)
            {
                var arr = new byte[PacketHeader.HeaderSize];

                headerBuffer = arr;
                headerReader = new GameBinaryReader(arr);
                headerWriter = new GameBinaryWriter(arr);
            }

            if (payloadBuffer.IsEmpty)
                payloadBuffer = new byte[PacketHeader.MaxPayloadSize];

            Socket from;
            Socket to;
            GameEncryptionSession fromEnc;
            GameEncryptionSession toEnc;

            if (direction == Direction.ClientToServer)
            {
                from = _clientSocket;
                to = _serverSocket;
                fromEnc = _clientEncryption;
                toEnc = _serverEncryption;
            }
            else
            {
                from = _serverSocket;
                to = _clientSocket;
                fromEnc = _serverEncryption;
                toEnc = _clientEncryption;
            }

            var toServer = to == _serverSocket;
            var fromServer = to == _serverSocket;

            bool DoReceive()
            {
                try
                {
                    ReceiveInternal(headerBuffer, from, fromEnc, fromServer);

                    var header = ReadHeader(headerReader);

                    if (header.Length > PacketHeader.MaxPayloadSize)
                    {
                        DisconnectInternal();
                        _log.Error("Disconnected client {0} from {1} due to invalid packet length: {2}",
                            EndPoint, Proxy.Info.Name, header.Length);
                        return false;
                    }

                    var payload = payloadBuffer.Slice(0, header.Length);

                    ReceiveInternal(payload, from, fromEnc, fromServer);

                    var original = payload;
                    var send = Proxy.InvokeReceived(this, direction, header.Code, ref payload);

                    if (send)
                    {
                        // A handler might have created a packet that's too large.
                        if (payload.Length > PacketHeader.MaxPayloadSize)
                        {
                            Proxy.Serializer.GameMessages.CodeToName.TryGetValue(header.Code,
                                out var name);

                            if (name == null)
                                name = header.Code.ToString();

                            _log.Error(
                                "{0}: Packet {1} is too large ({2} bytes) to be sent correctly; sending original",
                                direction.ToDirectionString(), name, payload.Length);

                            payload = original;
                        }

                        header = new PacketHeader((ushort)payload.Length, header.Code);

                        WriteHeader(headerWriter, header);

                        var headerSlice = headerBuffer.Slice(0, PacketHeader.HeaderSize);

                        SendInternal(headerSlice, true, to, toEnc, toServer);
                        SendInternal(payload, true, to, toEnc, toServer);
                    }
                }
                catch (SocketDisconnectedException)
                {
                    // Normal disconnection.
                    Disconnect();
                    return false;
                }
                catch (Exception e) when (IsSocketException(e))
                {
                    // The client is already disconnected.
                    return false;
                }

                return true;
            }

            // If we don't expect a large number of clients, just use dedicated
            // tasks to receive data instead of spawning a new one per receive.
            if (!nested && Proxy.MaxClients <= Environment.ProcessorCount)
                Task.Factory.StartNew(() =>
                {
                    while (DoReceive())
                    {
                    }

                    headerReader.Dispose();
                    headerWriter.Dispose();
                }, TaskCreationOptions.LongRunning);
            else
                Task.Run(() =>
                {
                    if (!DoReceive())
                    {
                        headerReader.Dispose();
                        headerWriter.Dispose();
                    }
                    else
                        Receive(direction, headerBuffer, headerReader, headerWriter, payloadBuffer, true);
                });
        }

        static bool IsSocketException(Exception exception)
        {
            return exception is SocketException ||
                exception is ObjectDisposedException;
        }

        void HandleException(Exception exception, bool server)
        {
            if (exception is SocketException sockEx)
                _log.Error("Disconnected {0} socket of client {1} from {2} due to error: {3}",
                    server ? "server" : "client", EndPoint, Proxy.Info.Name,
                    sockEx.SocketErrorCode.ToErrorString());
        }

        bool RunGuarded(bool rethrow, bool server, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e) when (IsSocketException(e))
            {
                if (!_disconnected)
                    HandleException(e, server);

                DisconnectInternal();

                if (rethrow)
                    throw;

                return false;
            }

            return true;
        }

        T RunGuarded<T>(bool rethrow, bool server, Func<T> action, Action<T> then)
        {
            T result;

            try
            {
                result = action();
            }
            catch (Exception e) when (IsSocketException(e))
            {
                if (!_disconnected)
                    HandleException(e, server);

                DisconnectInternal();

                if (rethrow)
                    throw;

                return default;
            }

            then?.Invoke(result);

            return result;
        }
    }
}
