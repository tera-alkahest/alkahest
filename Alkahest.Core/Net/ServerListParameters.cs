using System;
using System.Net;

namespace Alkahest.Core.Net
{
    public sealed class ServerListParameters
    {
        static readonly Uri _eu = new Uri(
            "http://web-sls.tera.gameforge.com:4566/servers/list.uk");

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

        public IPAddress RealAddress { get; }

        public IPAddress ServerListAddress { get; }

        public IPAddress GameAddress { get; }

        public int StartingPort { get; }

        public Region Region { get; }

        public TimeSpan Timeout { get; }

        public int Retries { get; }

        public Uri Uri { get; }

        public ServerListParameters(IPAddress realAddress, IPAddress slsAddress,
            IPAddress gameAddress, int startingPort, Region region,
            TimeSpan timeout, int retries)
        {
            RealAddress = realAddress;
            ServerListAddress = slsAddress;
            GameAddress = gameAddress;
            StartingPort = startingPort;
            Region = region;
            Timeout = timeout;
            Retries = retries;
            Uri = GetUri(region);
        }

        public static Uri GetUri(Region region)
        {
            switch (region)
            {
                case Region.EU:
                    return _eu;
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
                default:
                    throw Assert.Unreachable();
            }
        }
    }
}
