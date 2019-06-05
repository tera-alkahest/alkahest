using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_EDIT_PRIVATE_CHANNEL")]
    public sealed class CEditPrivateChannelPacket : SerializablePacket
    {
        public sealed class MemberInfo
        {
            public uint PlayerId { get; set; }
        }

        public List<MemberInfo> Members { get; } = new List<MemberInfo>();

        public string ChannelName { get; set; }

        public ushort Password { get; set; }
    }
}
