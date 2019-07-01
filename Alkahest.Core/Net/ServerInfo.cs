using System;
using System.Net;

namespace Alkahest.Core.Net
{
    public sealed class ServerInfo
    {
        public int Id { get; }

        public string Name { get; }

        public IPEndPoint RealEndPoint { get; }

        public IPEndPoint ProxyEndPoint { get; }

        internal ServerInfo(int id, string name, IPEndPoint realEndPoint, IPEndPoint proxyEndPoint)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RealEndPoint = realEndPoint ?? throw new ArgumentNullException(nameof(realEndPoint));
            ProxyEndPoint = proxyEndPoint ?? throw new ArgumentNullException(nameof(proxyEndPoint));
        }
    }
}
