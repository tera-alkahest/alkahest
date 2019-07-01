using Alkahest.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Alkahest.Core.Net.Game
{
    public sealed class GameProxy : IDisposable
    {
        static readonly Log _log = new Log(typeof(GameProxy));

        public ServerInfo Info { get; }

        internal ObjectPool<SocketAsyncEventArgs> ArgsPool { get; }

        public PacketProcessor Processor { get; }

        public int MaxClients { get; }

        public TimeSpan Timeout { get; }

        readonly HashSet<GameClient> _clients = new HashSet<GameClient>();

        readonly ManualResetEventSlim _event = new ManualResetEventSlim();

        readonly Socket _serverSocket;

        readonly int _backlog;

        bool _disposed;

        public GameProxy(ServerInfo info, ObjectPool<SocketAsyncEventArgs> pool,
            PacketProcessor processor, int backlog, int maxClients, TimeSpan timeout)
        {
            if (backlog < 0)
                throw new ArgumentOutOfRangeException(nameof(backlog));

            if (maxClients < 1)
                throw new ArgumentOutOfRangeException(nameof(maxClients));

            Info = info ?? throw new ArgumentNullException(nameof(info));
            ArgsPool = pool ?? throw new ArgumentNullException(nameof(pool));
            Processor = processor ?? throw new ArgumentNullException(nameof(processor));
            MaxClients = maxClients;
            Timeout = timeout;
            _serverSocket = new Socket(info.ProxyEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                ExclusiveAddressUse = true,
                NoDelay = true,
            };
            _backlog = backlog;
        }

        ~GameProxy()
        {
            RealDispose(false);
        }

        public void Dispose()
        {
            RealDispose(true);
            GC.SuppressFinalize(this);
        }

        void RealDispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_serverSocket == null)
                return;

            _serverSocket.SafeClose();
            _event.Wait();
            _event.Dispose();

            foreach (var client in _clients.ToArray())
                client.Disconnect();

            if (disposing)
                _log.Basic("Game proxy for {0} stopped", Info.Name);
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _serverSocket.Bind(Info.ProxyEndPoint);
            _serverSocket.Listen(_backlog);

            Accept();

            _log.Basic("Game proxy for {0} listening at {1}", Info.Name,
                Info.ProxyEndPoint);
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
                    _log.Info("Rejected connection to {0} from {1}: {2}", Info.Name, socket.RemoteEndPoint,
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

                _log.Info("Accepted connection to {0} from {1}", Info.Name, socket.RemoteEndPoint);
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
