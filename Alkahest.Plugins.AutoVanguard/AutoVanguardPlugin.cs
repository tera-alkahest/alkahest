using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Protocol.Packets;
using Alkahest.Core.Plugins;
using System.Linq;

namespace Alkahest.Plugins.AutoVanguard
{
    public sealed class AutoVanguardPlugin : IPlugin
    {
        public string Name { get; } = "autovanguard";

        static readonly Log _log = new Log(typeof(AutoVanguardPlugin));

        public void Start(GameProxy[] proxies)
        {
            foreach (var proc in proxies.Select(x => x.Processor))
                proc.AddHandler<SCompleteEventMatchingQuestPacket>(HandleVanguard);

            _log.Basic("Auto Vanguard plugin started");
        }

        public void Stop(GameProxy[] proxies)
        {
            foreach (var proc in proxies.Select(x => x.Processor))
                proc.RemoveHandler<SCompleteEventMatchingQuestPacket>(HandleVanguard);

            _log.Basic("Auto Vanguard plugin stopped");
        }

        bool HandleVanguard(GameClient client, Direction direction,
            SCompleteEventMatchingQuestPacket packet)
        {
            client.SendToServer(new CCompleteDailyEventPacket
            {
                Id = packet.Id
            });

            return true;
        }
    }
}
