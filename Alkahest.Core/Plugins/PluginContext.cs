using Alkahest.Core.Data;
using Alkahest.Core.Net.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Plugins
{
    public sealed class PluginContext
    {
        public DataCenter Data { get; }

        public IReadOnlyList<GameProxy> Proxies { get; }

        public PluginContext(DataCenter data, IEnumerable<GameProxy> proxies)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Proxies = (proxies ?? throw new ArgumentNullException(nameof(proxies))).ToArray();

            if (proxies.Any(x => x == null))
                throw new ArgumentException("A null proxy was given.", nameof(proxies));
        }
    }
}
