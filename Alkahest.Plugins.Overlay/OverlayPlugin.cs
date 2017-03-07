using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Plugins;

namespace Alkahest.Plugins.Overlay
{
    public sealed class OverlayPlugin : IPlugin
    {
        public string Name { get; } = "overlay";

        static readonly Log _log = new Log(typeof(OverlayPlugin));

        public void Start(GameProxy[] proxies)
        {
            _log.Basic("Overlay plugin started");
        }

        public void Stop(GameProxy[] proxies)
        {
            _log.Basic("Overlay plugin stopped");
        }
    }
}
