using System;
using System.Collections.Generic;
using System.Web.Http.SelfHost;
using Alkahest.Core.Logging;

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
            Parameters = parameters ??
                throw new ArgumentNullException(nameof(parameters));

            var ep = parameters.ProxyServerListEndPoint;
            var cfg = new HttpSelfHostConfiguration(
                $"http://{ep.Address}:{ep.Port}")
            {
                MessageHandlers =
                {
                    new ServerListRequestHandler(parameters, out var servers)
                }
            };

            Servers = servers;
            _server = new HttpSelfHostServer(cfg);
        }

        ~ServerListProxy()
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

            if (_server != null)
                Stop();
        }

        public void Start()
        {
            _server.OpenAsync().Wait();

            _log.Basic("{0} server list proxy listening at {1}",
                Parameters.Region,
                ((HttpSelfHostConfiguration)_server.Configuration).BaseAddress);
        }

        public void Stop()
        {
            _server.CloseAsync().Wait();

            _log.Basic("{0} server list proxy stopped", Parameters.Region);
        }
    }
}
