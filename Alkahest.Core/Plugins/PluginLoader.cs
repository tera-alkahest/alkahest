using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;

namespace Alkahest.Core.Plugins
{
    public sealed class PluginLoader
    {
        static readonly Log _log = new Log(typeof(PluginLoader));

        public IReadOnlyCollection<IPlugin> Plugins => _plugins;

        readonly IPlugin[] _plugins;

        public PluginLoader(string directory, string pattern, string[] exclude)
        {
            Directory.CreateDirectory(directory);

            using (var container = new CompositionContainer(
                new DirectoryCatalog(directory, pattern), true))
                    _plugins = container.GetExports<IPlugin>()
                        .Select(x => x.Value)
                        .Where(x => !exclude.Contains(x.Name))
                        .ToArray();
        }

        public void Start(GameProxy[] proxies)
        {
            foreach (var p in _plugins)
            {
                p.Start(proxies.ToArray());

                _log.Info("Started plugin {0}", p.Name);
            }

            _log.Basic("Started {0} plugins", _plugins.Length);
        }

        public void Stop(GameProxy[] proxies)
        {
            foreach (var p in _plugins)
            {
                p.Stop(proxies.ToArray());

                _log.Info("Stopped plugin {0}", p.Name);
            }
        }
    }
}
