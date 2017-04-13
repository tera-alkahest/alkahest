using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Protocol.Packets;
using Alkahest.Core.Plugins;
using System.Linq;

namespace Alkahest.Plugins.AutoNostrum
{
    public sealed class AutoNostrumPlugin : IPlugin
    {
        public string Name { get; } = "autonostrum";

        static readonly Log _log = new Log(typeof(AutoNostrumPlugin));

        public void Start(GameProxy[] proxies)
        {
            foreach (var proc in proxies.Select(x => x.Processor))
                proc.AddHandler<CReviveNowPacket>(HandleRevive);

            _log.Basic("Auto Nostrum plugin started");
        }

        public void Stop(GameProxy[] proxies)
        {
            foreach (var proc in proxies.Select(x => x.Processor))
                proc.RemoveHandler<CReviveNowPacket>(HandleRevive);

            _log.Basic("Auto Nostrum plugin stopped");
        }

        bool HandleRevive(GameClient client, Direction direction,
            CReviveNowPacket packet)
        {
            client.SendToServer(new CPCBangInventoryUseSlotPacket
            {
                Slot = 6
            });

            return true;
        }
    }
}
