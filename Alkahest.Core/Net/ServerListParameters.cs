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

        static readonly Uri _se = new Uri(
            "http://terasls.playwith.in.th/list.xml");

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
                { Region.SE, _se },
                { Region.TH, _th },
                { Region.TW, _tw },
                { Region.UK, _uk },
            };

        public IPAddress RealServerListAddress { get; }

        public IPEndPoint ProxyServerListEndPoint { get; }

        public IPAddress GameBaseAddress { get; }

        public int GamePort { get; }

        public Region Region { get; }

        public TimeSpan Timeout { get; }

        public int Retries { get; }

        public Uri Uri { get; }

        public ServerListParameters(IPAddress realSlsAddress, IPAddress proxySlsBaseAddress,
            int proxySlsPort, IPAddress gameBaseAddress, int baseGamePort, Region region,
            TimeSpan timeout, int retries)
        {
            if (proxySlsBaseAddress == null)
                throw new ArgumentNullException(nameof(proxySlsBaseAddress));

            // Match tera-proxy's port allocation scheme.
            var offset = GetRegionOffset(region);
            var port = baseGamePort + offset;

            if (baseGamePort < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(baseGamePort));

            if (baseGamePort > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(baseGamePort));

            if (retries < 0)
                throw new ArgumentOutOfRangeException(nameof(retries));

            // Match tera-proxy's IP allocation scheme. This obviously won't
            // work for IPv6...
            var ipBytes = proxySlsBaseAddress.GetAddressBytes();
            ipBytes[3] += (byte)offset;
            proxySlsBaseAddress = new IPAddress(ipBytes);

            Uri = Uris[region];
            RealServerListAddress = realSlsAddress ?? throw new ArgumentNullException(nameof(realSlsAddress));
            ProxyServerListEndPoint = new IPEndPoint(proxySlsBaseAddress, proxySlsPort);
            GameBaseAddress = gameBaseAddress ?? throw new ArgumentNullException(nameof(gameBaseAddress));
            GamePort = port;
            Region = region;
            Timeout = timeout;
            Retries = retries;
        }

        public static int GetRegionOffset(Region region)
        {
            switch (region.CheckValidity(nameof(region)))
            {
                case Region.DE:
                case Region.FR:
                case Region.UK:
                    return 0;
                case Region.RU:
                    return 2;
                case Region.TW:
                    return 3;
                case Region.JP:
                    return 4;
                case Region.SE:
                case Region.TH:
                    return 5;
                case Region.NA:
                    return 6;
                case Region.KR:
                    return 7;
                default:
                    throw Assert.Unreachable();
            }
        }
    }
}
