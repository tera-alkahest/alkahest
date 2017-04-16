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
            _server.OpenAsync().Wait();

            _log.Basic("Server list proxy listening at {0}", cfg.BaseAddress);
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

            _server?.CloseAsync().Wait();

            _log.Basic("Server list proxy stopped");
        }
    }
}
