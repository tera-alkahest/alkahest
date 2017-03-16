using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SDefendSuccessPacket : Packet
    {
        const string Name = "S_DEFEND_SUCCESS";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SDefendSuccessPacket();
        }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }

        [PacketField]
        public bool IsPerfectBlock { get; set; }
    }
}
