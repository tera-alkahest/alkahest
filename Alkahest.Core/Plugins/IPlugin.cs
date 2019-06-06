using Alkahest.Core.Net.Game;

namespace Alkahest.Core.Plugins
{
    public interface IPlugin
    {
        string Name { get; }

        void Start(PluginContext context, GameProxy[] proxies);

        void Stop(PluginContext context, GameProxy[] proxies);
    }
}
