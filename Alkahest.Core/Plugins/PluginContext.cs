using Alkahest.Core.Data;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Plugins
{
    public sealed class PluginContext
    {
        public Region Region { get; }

        public DataCenter Data { get; }

        public IReadOnlyList<GameProxy> Proxies { get; }

        public PacketSerializer Serializer { get; }

        public PacketDispatch Dispatch { get; }

        public PluginContext(Region region, DataCenter data, IEnumerable<GameProxy> proxies)
        {
            Region = region.CheckValidity(nameof(region));
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Proxies = (proxies ?? throw new ArgumentNullException(nameof(proxies))).ToArray();

            if (!proxies.Any())
                throw new ArgumentException("No proxies were given.");

            if (proxies.Any(x => x == null))
                throw new ArgumentException("A null proxy was given.", nameof(proxies));

            if (proxies.Select(x => x.Serializer).Distinct().Count() != 1)
                throw new ArgumentException("Proxies are not using the same serializer instance.");

            var proxy = proxies.First();

            Serializer = proxy.Serializer;
            Dispatch = new PacketDispatch(this);
        }
    }
}
