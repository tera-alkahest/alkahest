using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_BOSS_GAGE_INFO")]
    public sealed class SBossGaugeInfoPacket : SerializablePacket
    {
        public GameId Boss { get; set; }

        public uint HuntingZoneId { get; set; }

        public TemplateId Template { get; set; }

        public GameId Target { get; set; }

        public int Unknown1 { get; set; }

        public byte Unknown2 { get; set; }

        public ulong CurrentHP { get; set; }

        public ulong MaxHP { get; set; }

        public byte Unknown3 { get; set; }
    }
}
