using System.ComponentModel.Composition;
using Alkahest.Core.Net;

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
