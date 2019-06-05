using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SBossGaugeInfoPacket : Packet
    {
        const string Name = "S_BOSS_GAGE_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SBossGaugeInfoPacket();
        }

        [PacketField]
        public GameId Boss { get; set; }

        [PacketField]
        public uint HuntingZoneId { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public ulong CurrentHP { get; set; }

        [PacketField]
        public ulong MaxHP { get; set; }

        [PacketField]
        public byte Unknown3 { get; set; }
    }
}
