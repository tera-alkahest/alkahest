using System.Collections.Generic;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCreatePrivateChannelPacket : Packet
    {
        const string Name = "C_CREATE_PRIVATE_CHANNEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCreatePrivateChannelPacket();
        }

        public sealed class Member
        {
            [PacketField]
            public uint PlayerId { get; set; }
        }

        [PacketField]
        public List<Member> Members { get; } = new List<Member>();

        [PacketField]
        public string ChannelName { get; set; }

        [PacketField]
        public ushort Password { get; set; }
    }
}
