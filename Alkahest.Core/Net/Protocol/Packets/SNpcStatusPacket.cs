using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SNpcStatusPacket : Packet
    {
        const string Name = "S_NPC_STATUS";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SNpcStatusPacket();
        }

        [PacketField]
        public EntityId Source { get; set; }

        [PacketField]
        public bool IsEnraged { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }
    }
}
