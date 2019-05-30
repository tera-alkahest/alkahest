using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Packets;
using System.Linq;

namespace Alkahest.Scripts.Example
{
    public static class ExampleScript
    {
        static Log _log;

        static bool HandleCheckVersion(GameClient client, Direction direction, CCheckVersionPacket packet)
        {
            foreach (var ver in packet.Versions)
                _log.Info("Client reported version: {0}", ver.Value);

            return true;
        }

        // The special function __Start__ is invoked on startup. The proxies
        // parameter is an array of Alkahest.Core.Net.GameProxy instances. The
        // log parameter is an Alkahest.Core.Logging.Log instance created
        // specifically for this script package.
        public static void __Start__(GameProxy[] proxies, Log log)
        {
            _log = log;

            foreach (var proc in proxies.Select(x => x.Processor))
                proc.AddHandler<CCheckVersionPacket>(HandleCheckVersion);

            _log.Basic("Started example script");
        }

        // The special function __Stop__ is invoked on shutdown and receives the
        // same parameters that __Start__ did.
        public static void __Stop__(GameProxy[] proxies, Log log)
        {
            foreach (var proc in proxies.Select(x => x.Processor))
                proc.RemoveHandler<CCheckVersionPacket>(HandleCheckVersion);

            _log.Basic("Stopped example script");
        }
    }
}
