using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SAbnormalityFailPacket : Packet
    {
        const string Name = "S_ABNORMALITY_FAIL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAbnormalityFailPacket();
        }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public uint AbnormalityId { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public byte Unknown3 { get; set; }
    }
}
