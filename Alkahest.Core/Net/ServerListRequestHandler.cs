using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Alkahest.Core.Logging;

namespace Alkahest.Core.Net
{
    sealed class ServerListRequestHandler : DelegatingHandler
    {
        const string RemoteEndPointPropertyName =
            "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        static readonly Log _log = new Log(typeof(ServerListRequestHandler));

        readonly ServerListParameters _parameters;

        int _lastPort;

        readonly string _servers;

        public ServerListRequestHandler(ServerListParameters parameters,
            out IReadOnlyList<ServerInfo> servers)
        {
            _parameters = parameters;
            _lastPort = parameters.StartingPort;
            _servers = GetAndAdjustServers(out servers);
        }

        string GetAndAdjustServers(out IReadOnlyList<ServerInfo> servers)
        {
            _log.Basic("Fetching official server list...");

            HttpResponseMessage resp;

            using (var client = new HttpClient())
            {
                client.Timeout = _parameters.Timeout;

                var retriesSoFar = 0;

                while (true)
                {
                    var req = new HttpRequestMessage(HttpMethod.Get,
                        GetUri(_parameters.Uri.AbsolutePath, true));

                    req.Headers.Host = _parameters.Uri.Authority;

                    try
                    {
                        resp = client.SendAsync(req).Result.EnsureSuccessStatusCode();
                    }
                    catch (Exception e) when (IsHttpException(e))
                    {
                        if (retriesSoFar < _parameters.Retries)
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
            var servs = new List<ServerInfo>();

            foreach (var elem in doc.Root.Elements("server"))
            {
                var nameElem = elem.Element("name");
                var rawNameAttr = nameElem.Attribute("raw_name");
                var ipElem = elem.Element("ip");
                var portElem = elem.Element("port");

                var id = int.Parse(elem.Element("id").Value);
                var name = rawNameAttr.Value;
                var ip = IPAddress.Parse(ipElem.Value);
                var newIP = _parameters.GameAddress;
                var port = int.Parse(portElem.Value);
                var newPort = _lastPort++;

                nameElem.Value += " (Proxy)";
                rawNameAttr.Value += " (Proxy)";
                ipElem.Value = newIP.ToString();
                portElem.Value = newPort.ToString();

                servs.Add(new ServerInfo(id, name, new IPEndPoint(ip, port),
                    new IPEndPoint(newIP, newPort)));

                _log.Info("Redirected {0}: {1}:{2} -> {3}:{4}",
                    name, ip, port, newIP, newPort);
            }

            servers = servs.OrderBy(x => x.Id).ToArray();

            _log.Basic("Redirected {0} servers", servs.Count);

            return doc.ToString();
        }

        static bool IsHttpException(Exception exception)
        {
            return exception is AggregateException ||
                exception is HttpRequestException;
        }

        Uri GetUri(string path, bool usn)
        {
            var uri = $"http://{_parameters.RealServerListAddress}:{_parameters.Uri.Port}{path}";

            if (usn && _parameters.Region == Region.JP)
                uri += "?usn=0";

            return new Uri(uri);
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var p = (RemoteEndpointMessageProperty)
                request.Properties[RemoteEndPointPropertyName];
            var from = $"{p.Address}:{p.Port}";
            var uri = request.RequestUri;
            var path = uri.PathAndQuery;

            _log.Debug("Received HTTP request at {0} from {1}", path, from);

            return base.SendAsync(request, cancellationToken).ContinueWith(t =>
            {
                HttpResponseMessage resp;

                using (var client = new HttpClient())
                {
                    client.Timeout = _parameters.Timeout;

                    var retriesSoFar = 0;

                    while (true)
                    {
                        // We need to make a new request object or we'll get an
                        // exception when attempting to send it below.
                        var req = new HttpRequestMessage(
                            request.Method, GetUri(path, false))
                        {
                            Version = request.Version
                        };

                        req.Headers.Clear();

                        foreach (var hdr in request.Headers)
                            req.Headers.Add(hdr.Key, hdr.Value);

                        req.Headers.Host = _parameters.Uri.Authority;

                        var content = request.Content.ReadAsByteArrayAsync().Result;

                        if (content.Length != 0)
                            req.Content = new ByteArrayContent(content);

                        try
                        {
                            resp = client.SendAsync(req).Result;
                        }
                        catch (AggregateException)
                        {
                            if (retriesSoFar < _parameters.Retries)
                            {
                                _log.Error("Could not forward HTTP request at {0} from {1}, retrying...",
                                    path, from);
                                retriesSoFar++;
                                continue;
                            }

                            _log.Error("Gave up forwarding HTTP request at {0} from {1} after {2} retries",
                                path, from, _parameters.Retries);

                            // The official server seems dead.
                            return new HttpResponseMessage(HttpStatusCode.GatewayTimeout);
                        }

                        break;
                    }
                }

                resp.Headers.TransferEncodingChunked = null;

                // Don't include the query string as TERA JP actually uses it.
                if (uri.AbsolutePath == _parameters.Uri.AbsolutePath)
                    resp.Content = new StringContent(_servers);

                _log.Debug("Forwarded HTTP request at {0} from {1}: {2}",
                    path, from, (int)resp.StatusCode);

                return resp;
            });
        }
    }
}
