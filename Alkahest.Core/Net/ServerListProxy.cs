using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using System.Xml.Linq;
using Alkahest.Core.Logging;

namespace Alkahest.Core.Net
{
    public sealed class ServerListProxy : IDisposable
    {
        sealed class ServerListRequestHandler : DelegatingHandler
        {
            const string RemoteEndPointPropertyName =
                "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

            static readonly Log _log = new Log(typeof(ServerListRequestHandler));

            readonly ServerListParameters _params;

            readonly string _hostHeader;

            readonly string _serverListPath;

            readonly string _servers;

            public ServerListRequestHandler(ServerListProxy proxy)
            {
                _params = proxy.Parameters;
                _hostHeader = $"{_params.Host}:{_params.Port}";
                _serverListPath = $"/servers/list.{_params.Region.ToRegionString()}";
                _servers = GetAndAdjustServers(proxy);
            }

            string GetAndAdjustServers(ServerListProxy proxy)
            {
                _log.Basic("Fetching official server list...");

                HttpResponseMessage resp;

                using (var client = new HttpClient())
                {
                    client.Timeout = _params.Timeout;

                    var retriesSoFar = 0;

                    while (true)
                    {
                        var req = new HttpRequestMessage(HttpMethod.Get,
                            GetUri(_serverListPath));

                        req.Headers.Host = _hostHeader;

                        try
                        {
                            resp = client.SendAsync(req).Result.EnsureSuccessStatusCode();
                        }
                        catch (AggregateException)
                        {
                            if (retriesSoFar < _params.Retries)
                            {
                                _log.Error("Could not fetch official server list, retrying...");
                                retriesSoFar++;
                                continue;
                            }

                            throw;
                        }

                        break;
                    }
                }

                _log.Basic("Fetched official server list");

                var doc = XDocument.Parse(resp.Content.ReadAsStringAsync().Result);
                var servers = new List<ServerInfo>();

                foreach (var elem in doc.Root.Elements("server"))
                {
                    var ipElem = elem.Element("ip");

                    var id = int.Parse(elem.Element("id").Value);
                    var name = elem.Element("name").Value;
                    var cat = elem.Element("category").Value;
                    var ip = IPAddress.Parse(ipElem.Value);
                    var port = int.Parse(elem.Element("port").Value);
                    var newIP = _params.GameAddress;

                    ipElem.Value = newIP.ToString();

                    servers.Add(new ServerInfo(id, name, cat, ip, newIP, port));

                    _log.Info("Redirected {0}: {1}:{2} -> {3}:{2}",
                        name, ip, port, newIP);
                }

                proxy.Servers = servers;

                _log.Basic("Redirected {0} servers", servers.Count);

                return doc.ToString();
            }

            Uri GetUri(string path)
            {
                return new Uri($"http://{_params.RealAddress}:{_params.Port}{path}");
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var p = (RemoteEndpointMessageProperty)
                    request.Properties[RemoteEndPointPropertyName];
                var from = $"{p.Address}:{p.Port}";
                var path = request.RequestUri.PathAndQuery;

                _log.Debug("Received HTTP request at {0} from {1}", path, from);

                return base.SendAsync(request, cancellationToken).ContinueWith(t =>
                {
                    HttpResponseMessage resp;

                    using (var client = new HttpClient())
                    {
                        client.Timeout = _params.Timeout;

                        var retriesSoFar = 0;

                        while (true)
                        {
                            // We need to make a new request object or we'll get an
                            // exception when attempting to send it below.
                            var req = new HttpRequestMessage(
                                request.Method, GetUri(path))
                            {
                                Version = request.Version
                            };

                            req.Headers.Clear();

                            foreach (var hdr in request.Headers)
                                req.Headers.Add(hdr.Key, hdr.Value);

                            req.Headers.Host = _hostHeader;

                            var content = request.Content.ReadAsByteArrayAsync().Result;

                            if (content.Length != 0)
                                req.Content = new ByteArrayContent(content);

                            try
                            {
                                resp = client.SendAsync(req).Result;
                            }
                            catch (AggregateException)
                            {
                                if (retriesSoFar < _params.Retries)
                                {
                                    _log.Error("Could not forward HTTP request at {0} from {1}, retrying...",
                                        path, from);
                                    retriesSoFar++;
                                    continue;
                                }

                                _log.Error("Gave up forwarding HTTP request at {0} from {1} after {2} retries",
                                    path, from, _params.Retries);

                                // The official server seems dead.
                                return new HttpResponseMessage(HttpStatusCode.GatewayTimeout);
                            }

                            break;
                        }
                    }

                    if (path == _serverListPath)
                        resp.Content = new StringContent(_servers);

                    _log.Debug("Forwarded HTTP request at {0} from {1}: {2}",
                        path, from, (int)resp.StatusCode);

                    return resp;
                });
            }
        }

        static readonly Log _log = new Log(typeof(ServerListProxy));

        public ServerListParameters Parameters { get; }

        public IReadOnlyList<ServerInfo> Servers { get; private set; }

        readonly HttpSelfHostServer _server;

        bool _disposed;

        public ServerListProxy(ServerListParameters parameters)
        {
            Parameters = parameters;

            var cfg = new HttpSelfHostConfiguration(
                $"http://{parameters.ServerListAddress}:{parameters.Port}");
            cfg.MessageHandlers.Add(new ServerListRequestHandler(this));

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

            _server.CloseAsync().Wait();

            _log.Basic("Server list proxy stopped");
        }
    }
}
