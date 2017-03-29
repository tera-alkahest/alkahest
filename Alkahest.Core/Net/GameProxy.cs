using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Protocol;

namespace Alkahest.Core.Net
{
    public sealed class GameProxy : IDisposable
    {
        static readonly Log _log = new Log(typeof(GameProxy));

        public ServerInfo Info { get; }

        internal ObjectPool<SocketAsyncEventArgs> ArgsPool { get; }

        public PacketProcessor Processor { get; }

        public int MaxClients { get; set; }

        public TimeSpan Timeout { get; }

        public IPEndPoint RealEndPoint { get; }

        public IPEndPoint ProxyEndPoint { get; }

        readonly HashSet<GameClient> _clients = new HashSet<GameClient>();

        readonly ManualResetEventSlim _event = new ManualResetEventSlim();

        readonly object _lock = new object();

        readonly Socket _serverSocket;

        bool _disposed;

        public GameProxy(ServerInfo info, ObjectPool<SocketAsyncEventArgs> pool,
            PacketProcessor processor, int backlog, TimeSpan timeout)
        {
            Info = info;
            ArgsPool = pool;
            Processor = processor;
            Timeout = timeout;
            RealEndPoint = new IPEndPoint(info.RealAddress, info.Port);
            ProxyEndPoint = new IPEndPoint(info.ProxyAddress, info.Port);
            _serverSocket = new Socket(ProxyEndPoint.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp)
            {
                ExclusiveAddressUse = true,
                NoDelay = true
            };

            _serverSocket.Bind(ProxyEndPoint);
            _serverSocket.Listen(backlog);

            _log.Basic("Game proxy for {0} listening at {1}", info.Name, ProxyEndPoint);

            Accept();
        }

        ~GameProxy()
        {
            RealDispose();
        }

        public void Dispose()
        {
            RealDispose();
            GC.SuppressFinalize(this);
        }

        void RealDispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _serverSocket.SafeClose();
            _event.Wait();

            foreach (var client in _clients.ToArray())
                client.Disconnect();

            _log.Basic("Game proxy for {0} stopped", Info.Name);
        }

        void Accept()
        {
            var args = ArgsPool.Get();
            args.Completed += OnAccept;

            Task.Factory.StartNew(() =>
            {
                if (!_serverSocket.AcceptAsync(args))
                    OnAccept(this, args);
            }, TaskCreationOptions.LongRunning);
        }

        void OnAccept(object sender, SocketAsyncEventArgs args)
        {
            var error = args.SocketError;
            var socket = args.AcceptSocket;
            var reject = _clients.Count >= MaxClients;

            args.Completed -= OnAccept;
            ArgsPool.TryPut(args);

            if (error != SocketError.Success || reject)
            {
                var isAbort = error == SocketError.OperationAborted;

                if (!isAbort)
                    _log.Info("Rejected connection to {0} from {1}: {2}",
                        Info.Name, socket.RemoteEndPoint,
                        reject ? "Maximum amount of clients reached" : error.ToErrorString());

                socket.SafeClose();

                // Are we shutting down?
                if (isAbort)
                {
                    _event.Set();
                    return;
                }
            }
            else
            {
                var client = new GameClient(this, socket);

                _log.Info("Accepted connection to {0} from {1}",
                    Info.Name, socket.RemoteEndPoint);
            }

            Accept();
        }

        internal void AddClient(GameClient client)
        {
            lock (_clients)
                _clients.Add(client);
        }

        internal void RemoveClient(GameClient client)
        {
            lock (_clients)
                _clients.Remove(client);
        }
    }
}
