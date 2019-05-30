using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SHeldAbnormalityAddPacket : Packet
    {
        const string Name = "S_HOLD_ABNORMALITY_ADD";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SHeldAbnormalityAddPacket();
        }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint AbnormalityId { get; set; }

        [PacketField]
        public uint Duration { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }
    }
}
