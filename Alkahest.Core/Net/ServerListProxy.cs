using Alkahest.Core.Logging;
using System;
using System.Collections.Generic;
using System.Web.Http.SelfHost;

namespace Alkahest.Core.Net
{
    public sealed class ServerListProxy : IDisposable
    {
        static readonly Log _log = new Log(typeof(ServerListProxy));

        public ServerListParameters Parameters { get; }

        public IReadOnlyList<ServerInfo> Servers { get; }

        readonly HttpSelfHostServer _server;

        bool _disposed;

        public ServerListProxy(ServerListParameters parameters)
        {
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            var ep = parameters.ProxyServerListEndPoint;
            var cfg = new HttpSelfHostConfiguration($"{parameters.Uri.Scheme}://{ep.Address}:{ep.Port}")
            {
                MessageHandlers =
                {
                    new ServerListRequestHandler(parameters, out var servers),
                },
            };

            Servers = servers;
            _server = new HttpSelfHostServer(cfg);
        }

        ~ServerListProxy()
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

            if (_server == null)
                return;

            _server.CloseAsync().Wait();

            if (disposing)
                _log.Basic("{0} server list proxy stopped", Parameters.Region);
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _server.OpenAsync().Wait();

            _log.Basic("{0} server list proxy listening at {1}", Parameters.Region,
                ((HttpSelfHostConfiguration)_server.Configuration).BaseAddress);
        }
    }
}
