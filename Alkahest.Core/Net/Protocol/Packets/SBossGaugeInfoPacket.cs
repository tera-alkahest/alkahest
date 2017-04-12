using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
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
        public EntityId Boss { get; set; }

        [PacketField]
        public uint HuntingZoneId { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public float HPDifference { get; set; }

        [PacketField]
        public bool IsEnraged { get; set; }

        [PacketField]
        public float CurrentHP { get; set; }

        [PacketField]
        public float MaxHP { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
