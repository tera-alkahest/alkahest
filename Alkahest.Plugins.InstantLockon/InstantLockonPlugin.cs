using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Protocol.Packets;
using Alkahest.Core.Plugins;
using System.Linq;

namespace Alkahest.Plugins.InstantLockon
{
    public sealed class InstantLockonPlugin : IPlugin
    {
        public string Name { get; } = "instantlockon";

        static readonly Log _log = new Log(typeof(InstantLockonPlugin));

        public void Start(GameProxy[] proxies)
        {
            foreach (var proc in proxies.Select(x => x.Processor))
                proc.AddHandler<CCanLockOnTargetPacket>(HandleLockon);

            _log.Basic("Instant Lockon plugin started");
        }

        public void Stop(GameProxy[] proxies)
        {
            foreach (var proc in proxies.Select(x => x.Processor))
                proc.RemoveHandler<CCanLockOnTargetPacket>(HandleLockon);

            _log.Basic("Instant Lockon plugin stopped");
        }

        bool HandleLockon(GameClient client, Direction direction,
            CCanLockOnTargetPacket packet)
        {
            client.SendToClient(new SCanLockOnTargetPacket
            {
                CanLockOn = true,
                Skill = packet.Skill,
                Target = packet.Target,
                Unknown1 = packet.Unknown1
            });

            return true;
        }
    }
}
