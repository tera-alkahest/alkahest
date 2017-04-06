using System.Net;

namespace Alkahest.Core.Net
{
    public sealed class ServerInfo
    {
        public int Id { get; }

        public string Name { get; }

        public IPEndPoint RealEndPoint { get; }

        public IPEndPoint ProxyEndPoint { get; }

        public ServerInfo(int id, string name, IPEndPoint realEndPoint,
            IPEndPoint proxyEndPoint)
        {
            Id = id;
            Name = name;
            RealEndPoint = realEndPoint;
            ProxyEndPoint = proxyEndPoint;
        }
    }
}
