using System;
using System.Net;

namespace Alkahest.Core.Net
{
    public sealed class ServerListParameters
    {
        public IPAddress RealAddress { get; }

        public IPAddress ServerListAddress { get; }

        public IPAddress GameAddress { get; }

        public Region Region { get; }

        public TimeSpan Timeout { get; }

        public int Retries { get; }

        public string Host => GetHost(Region);

        public int Port => GetPort(Region);

        public ServerListParameters(IPAddress realAddress, IPAddress slsAddress,
            IPAddress gameAddress, Region region, TimeSpan timeout, int retries)
        {
            RealAddress = realAddress;
            ServerListAddress = slsAddress;
            GameAddress = gameAddress;
            Region = region;
            Timeout = timeout;
            Retries = retries;
        }

        public static string GetHost(Region region)
        {
            switch (region)
            {
                case Region.EU:
                    return "web-sls.tera.gameforge.com";
                case Region.NA:
                    return "sls.service.enmasse.com";
                default:
                    throw Assert.Unreachable();
            }
        }

        public static int GetPort(Region region)
        {
            switch (region)
            {
                case Region.EU:
                    return 4566;
                case Region.NA:
                    return 4566;
                default:
                    throw Assert.Unreachable();
            }
        }
    }
}
