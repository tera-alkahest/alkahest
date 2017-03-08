using System.Net;

namespace Alkahest.Core.Net
{
    public sealed class ServerInfo
    {
        public int Id { get; }

        public string Name { get; }

        public string Category { get; }

        public IPAddress RealAddress { get; }

        public IPAddress ProxyAddress { get; }

        public int Port { get; }

        internal ServerInfo(int id, string name, string category,
            IPAddress realAddress, IPAddress proxyAddress, int port)
        {
            Id = id;
            Name = name;
            Category = category;
            RealAddress = realAddress;
            ProxyAddress = proxyAddress;
            Port = port;
        }
    }
}
