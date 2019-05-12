using System;
using System.Collections.Generic;
using System.Net;

namespace Alkahest.Core.Net
{
    public sealed class ServerListParameters
    {
        static readonly Uri _de = new Uri(
            "https://serverlist.tera.gameforge.com:443/serverlist/xml/default/de");

        static readonly Uri _fr = new Uri(
            "https://serverlist.tera.gameforge.com:443/serverlist/xml/default/fr");

        static readonly Uri _jp = new Uri(
            "https://tera.pmang.jp:443/game_launcher/server_list.xml");

        static readonly Uri _kr = new Uri(
            "http://tera.nexon.com:80/launcher/sls/servers/list.xml");

        static readonly Uri _na = new Uri(
            "http://sls.service.enmasse.com:8080/servers/list.en");

        static readonly Uri _ru = new Uri(
            "http://launcher.tera-online.ru:80/launcher/sls/");

        static readonly Uri _th = new Uri(
            "http://terasls.playwith.in.th/list.xml");

        static readonly Uri _tw = new Uri(
            "http://tera.mangot5.com:80/game/tera/serverList.xml");

        static readonly Uri _uk = new Uri(
            "https://serverlist.tera.gameforge.com:443/serverlist/xml/default/uk");

        public static IReadOnlyDictionary<Region, Uri> Uris { get; } =
            new Dictionary<Region, Uri>
            {
                { Region.DE, _de },
                { Region.FR, _fr },
                { Region.JP, _jp },
                { Region.KR, _kr },
                { Region.NA, _na },
                { Region.RU, _ru },
                { Region.TH, _th },
                { Region.TW, _tw },
                { Region.UK, _uk },
            };

        public IPAddress RealServerListAddress { get; }

        public IPEndPoint ProxyServerListEndPoint { get; }

        public IPAddress GameAddress { get; }

        public int BasePort { get; }

        public Region Region { get; }

        public TimeSpan Timeout { get; }

        public int Retries { get; }

        public Uri Uri { get; }

        public ServerListParameters(IPAddress realSlsAddress,
            IPAddress proxySlsAddress, int proxySlsPort, IPAddress gameAddress,
            int basePort, Region region, TimeSpan timeout, int retries)
        {
            if (basePort < IPEndPoint.MinPort || basePort >= IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(basePort));

            region.CheckValidity(nameof(region));

            if (retries < 0)
                throw new ArgumentOutOfRangeException(nameof(retries));

            Uri = Uris[region];
            RealServerListAddress = realSlsAddress ??
                throw new ArgumentNullException(nameof(realSlsAddress));
            ProxyServerListEndPoint = new IPEndPoint(
                proxySlsAddress ?? throw new ArgumentNullException(nameof(proxySlsAddress)),
                proxySlsPort);
            GameAddress = gameAddress ?? throw new ArgumentNullException(nameof(gameAddress));
            BasePort = basePort;
            Region = region;
            Timeout = timeout;
            Retries = retries;
        }
    }
}
