using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Alkahest.Core.Cryptography;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Protocol;

namespace Alkahest.Core.Net
{
    public sealed class GameClient
    {
        static readonly byte[] _magic = new byte[] { 0x01, 0x00, 0x00, 0x00 };

        static readonly Log _log = new Log(typeof(GameClient));

        public GameProxy Proxy { get; }

        public IPEndPoint EndPoint { get; }

        readonly Socket _clientSocket;

        TeraEncryptionSession _clientEncryption;

        readonly byte[] _clientSendBuffer =
            new byte[PacketHeader.MaxPacketSize];

        readonly Socket _serverSocket;

        TeraEncryptionSession _serverEncryption;

        readonly byte[] _serverSendBuffer =
            new byte[PacketHeader.MaxPacketSize];

        volatile bool _disconnected;

        internal GameClient(GameProxy proxy, Socket socket)
        {
            Proxy = proxy;
            EndPoint = (IPEndPoint)socket.RemoteEndPoint;
            _clientSocket = socket;
            _serverSocket = new Socket(proxy.RealEndPoint.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            SetOptions();

            var args = proxy.ArgsPool.Get();
            args.Completed += OnConnect;
            args.RemoteEndPoint = Proxy.RealEndPoint;

            proxy.AddClient(this);

            Connect(args);
        }

        void SetOptions()
        {
            var timeout = (int)Proxy.Timeout.TotalMilliseconds;

            foreach (var socket in new[] { _clientSocket, _serverSocket })
            {
                socket.NoDelay = true;
                socket.ReceiveTimeout = timeout;
                socket.SendTimeout = timeout;
            }
        }

        public void Disconnect()
        {
            DisconnectInternal();

            _log.Info("Disconnected client {0} from {1} ({2})",
                EndPoint, Proxy.Info.Name, Proxy.RealEndPoint);
        }

        void DisconnectInternal()
        {
            Proxy.RemoveClient(this);

            _disconnected = true;

            _serverSocket.SafeClose();
            _clientSocket.SafeClose();
        }

        public bool SendToClient(RawPacket packet)
        {
            return SendRawPacketInternal(packet, _clientSendBuffer,
                _clientSocket, _clientEncryption, false);
        }

        public bool SendToServer(RawPacket packet)
        {
            return SendRawPacketInternal(packet, _serverSendBuffer,
                _serverSocket, _serverEncryption, true);
        }

        bool SendRawPacketInternal(RawPacket packet, byte[] buffer,
            Socket socket, TeraEncryptionSession encryption, bool server)
        {
            lock (socket)
            {
                var header = new PacketHeader((ushort)packet.Payload.Length,
                    Proxy.Processor.Serializer.GameMessages.NameToOpCode[packet.OpCode]);

                PacketProcessor.WriteHeader(header, buffer);
                Buffer.BlockCopy(packet.Payload, 0, buffer,
                    PacketHeader.HeaderSize, header.Length);

                try
                {
                    return SendInternal(buffer, header.FullLength, false,
                        socket, encryption, server);
                }
                catch (SocketDisconnectedException)
                {
                    // Normal disconnection.
                    return false;
                }
            }
        }

        public bool SendToClient(Packet packet)
        {
            return SendPacketInternal(packet, _clientSendBuffer,
                _clientSocket, _clientEncryption, false);
        }

        public bool SendToServer(Packet packet)
        {
            return SendPacketInternal(packet, _serverSendBuffer,
                _serverSocket, _serverEncryption, true);
        }

        bool SendPacketInternal(Packet packet, byte[] buffer, Socket socket,
            TeraEncryptionSession encryption, bool server)
        {
            lock (socket)
            {
                var data = Proxy.Processor.Serializer.Serialize(packet);
                var header = new PacketHeader((ushort)data.Length,
                    Proxy.Processor.Serializer.GameMessages.NameToOpCode[packet.OpCode]);

                PacketProcessor.WriteHeader(header, buffer);
                Buffer.BlockCopy(data, 0, buffer, PacketHeader.HeaderSize,
                    header.Length);

                try
                {
                    return SendInternal(buffer, header.FullLength, false,
                        socket, encryption, server);
                }
                catch (SocketDisconnectedException)
                {
                    // Normal disconnection.
                    return false;
                }
            }
        }

        bool SendToClientInternal(byte[] data, bool rethrow)
        {
            return SendInternal(data, data.Length, rethrow,
                _clientSocket, _clientEncryption, false);
        }

        bool SendToServerInternal(byte[] data, bool rethrow)
        {
            return SendInternal(data, data.Length, rethrow,
                _serverSocket, _serverEncryption, true);
        }

        bool SendInternal(byte[] data, int length, bool rethrow,
            Socket socket, TeraEncryptionSession encryption, bool server)
        {
            return RunGuarded(rethrow, server, () =>
            {
                if (encryption != null)
                    lock (encryption)
                        encryption.Encrypt(data, 0, length);

                lock (socket)
                    socket.SendFull(data, 0, length);
            });
        }

        byte[] ReceiveFromClientInternal(int length)
        {
            var data = new byte[length];

            ReceiveInternal(data, data.Length, _clientSocket,
                _clientEncryption, false);

            return data;
        }

        byte[] ReceiveFromServerInternal(int length)
        {
            var data = new byte[length];

            ReceiveInternal(data, data.Length, _serverSocket,
                _serverEncryption, true);

            return data;
        }

        void ReceiveInternal(byte[] data, int length, Socket socket,
            TeraEncryptionSession encryption, bool server)
        {
            RunGuarded(true, server, () =>
            {
                socket.ReceiveFull(data, 0, length);

                if (encryption != null)
                    lock (encryption)
                        encryption.Decrypt(data, 0, length);
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
                _log.Error("Could not connect to {0} ({1}) for client {2}: {3}",
                    Proxy.Info.Name, Proxy.RealEndPoint, EndPoint, error);
                return;
            }

            _log.Info("Connected to {0} ({1}) for client {2}",
                Proxy.Info.Name, Proxy.RealEndPoint, EndPoint);

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
                        EndPoint, Proxy.Info.Name,
                        magic.Aggregate("0x", (acc, x) => acc + x.ToString("X2")));
                    return;
                }

                ckey1 = ReceiveFromClientInternal(TeraEncryptionSession.KeySize);
                SendToServerInternal(ckey1, true);

                skey1 = ReceiveFromServerInternal(TeraEncryptionSession.KeySize);
                SendToClientInternal(skey1, true);

                ckey2 = ReceiveFromClientInternal(TeraEncryptionSession.KeySize);
                SendToServerInternal(ckey2, true);

                skey2 = ReceiveFromServerInternal(TeraEncryptionSession.KeySize);
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

            _clientEncryption = new TeraEncryptionSession(
                Direction.ClientToServer, ckey1, ckey2, skey1, skey2);
            _serverEncryption = new TeraEncryptionSession(
                Direction.ServerToClient, ckey1, ckey2, skey1, skey2);

            _log.Info("Established encrypted session for client {0}", EndPoint);

            Receive(Direction.ClientToServer, null, null);
            Receive(Direction.ServerToClient, null, null);
        }

        void Receive(Direction direction, byte[] headerBuffer, byte[] payloadBuffer)
        {
            headerBuffer = headerBuffer ?? new byte[PacketHeader.HeaderSize];
            payloadBuffer = payloadBuffer ?? new byte[PacketHeader.MaxPayloadSize];

            Socket from;
            Socket to;
            TeraEncryptionSession fromEnc;
            TeraEncryptionSession toEnc;

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
                    ReceiveInternal(headerBuffer, PacketHeader.HeaderSize,
                        from, fromEnc, fromServer);

                    var header = PacketProcessor.ReadHeader(headerBuffer);

                    ReceiveInternal(payloadBuffer, header.Length,
                        from, fromEnc, fromServer);

                    // Can be set to a new array.
                    var payload = payloadBuffer;

                    if (Proxy.Processor.Process(this, direction, ref header, ref payload))
                    {
                        PacketProcessor.WriteHeader(header, headerBuffer);

                        SendInternal(headerBuffer, headerBuffer.Length,
                            true, to, toEnc, toServer);
                        SendInternal(payload, header.Length,
                            true, to, toEnc, toServer);
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
            if (Proxy.MaxClients <= Environment.ProcessorCount)
            {
                Task.Factory.StartNew(() =>
                {
                    while (DoReceive())
                    {
                    }
                }, TaskCreationOptions.LongRunning);
            }
            else
            {
                Task.Run(() =>
                {
                    DoReceive();

                    Receive(direction, headerBuffer, payloadBuffer);
                });
            }
        }

        static bool IsSocketException(Exception exception)
        {
            return exception is SocketException || exception is ObjectDisposedException;
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

                return default(T);
            }

            then?.Invoke(result);

            return result;
        }
    }
}
