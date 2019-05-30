using Alkahest.Core.Net.Game;
using System.ComponentModel.Composition;

namespace Alkahest.Core.Plugins
{
    [InheritedExport(typeof(IPlugin))]
    public interface IPlugin
    {
        string Name { get; }

        void Start(GameProxy[] proxies);

        void Stop(GameProxy[] proxies);
    }
}
