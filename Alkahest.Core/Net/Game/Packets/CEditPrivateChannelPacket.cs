using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CEditPrivateChannelPacket : Packet
    {
        const string Name = "C_EDIT_PRIVATE_CHANNEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CEditPrivateChannelPacket();
        }

        public sealed class MemberInfo
        {
            [PacketField]
            public uint PlayerId { get; set; }
        }

        [PacketField]
        public List<MemberInfo> Members { get; } = new List<MemberInfo>();

        [PacketField]
        public string ChannelName { get; set; }

        [PacketField]
        public ushort Password { get; set; }
    }
}
