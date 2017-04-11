using System.Collections.Generic;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCrestApplyListPacket : Packet
    {
        const string Name = "C_CREST_APPLY_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCrestApplyListPacket();
        }

        public sealed class CrestInfo
        {
            [PacketField]
            public uint CrestId { get; set; }
        }

        [PacketField]
        public List<CrestInfo> Crests { get; } = new List<CrestInfo>();

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
