using System;
using System.Net;

namespace Alkahest.Core.Net
{
    public sealed class ServerListParameters
    {
        static readonly Uri _de = new Uri(
            "http://web-sls.tera.gameforge.com:4566/servers/list.de");

        static readonly Uri _fr = new Uri(
            "http://web-sls.tera.gameforge.com:4566/servers/list.fr");

        static readonly Uri _jp = new Uri(
            "http://tera.pmang.jp:80/game_launcher/server_list.xml");

        static readonly Uri _kr = new Uri(
            "http://tera.nexon.com:80/launcher/sls/servers/list.xml");

        static readonly Uri _na = new Uri(
            "http://sls.service.enmasse.com:8080/servers/list.en");

        static readonly Uri _ru = new Uri(
            "http://launcher.tera-online.ru:80/launcher/sls/");

        static readonly Uri _tw = new Uri(
            "http://tera.mangot5.com:80/game/tera/serverList.xml");

        static readonly Uri _uk = new Uri(
            "http://web-sls.tera.gameforge.com:4566/servers/list.uk");

        public IPAddress RealServerListAddress { get; }

        public IPEndPoint ProxyServerListEndPoint { get; }

        public IPAddress GameAddress { get; }

        public int BasePort { get; }

        public Region Region { get; }

        public TimeSpan Timeout { get; }

        public int Retries { get; }

        public Uri Uri { get; }

        public ServerListParameters(IPAddress realSlsAddress,
            IPAddress proxySlsAddress, int? proxySlsPort,
            IPAddress gameAddress, int basePort, Region region,
            TimeSpan timeout, int retries)
        {
            if (basePort < IPEndPoint.MinPort ||
                basePort >= IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(basePort));

            region.CheckValidity(nameof(region));

            if (retries < 0)
                throw new ArgumentOutOfRangeException(nameof(retries));

            Uri = GetUri(region);
            RealServerListAddress = realSlsAddress ??
                throw new ArgumentNullException(nameof(realSlsAddress));
            ProxyServerListEndPoint = new IPEndPoint(
                proxySlsAddress ?? throw new ArgumentNullException(nameof(proxySlsAddress)),
                proxySlsPort ?? Uri.Port);
            GameAddress = gameAddress ??
                throw new ArgumentNullException(nameof(gameAddress));
            BasePort = basePort;
            Region = region;
            Timeout = timeout;
            Retries = retries;
        }

        public static Uri GetUri(Region region)
        {
            region.CheckValidity(nameof(region));

            switch (region)
            {
                case Region.DE:
                    return _de;
                case Region.FR:
                    return _fr;
                case Region.JP:
                    return _jp;
                case Region.KR:
                    return _kr;
                case Region.NA:
                    return _na;
                case Region.RU:
                    return _ru;
                case Region.TW:
                    return _tw;
                case Region.UK:
                    return _uk;
                default:
                    throw Assert.Unreachable();
            }
        }
    }
}
