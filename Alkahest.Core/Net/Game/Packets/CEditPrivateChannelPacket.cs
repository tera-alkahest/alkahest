using Alkahest.Core.Collections;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_EDIT_PRIVATE_CHANNEL")]
    public sealed class CEditPrivateChannelPacket : SerializablePacket
    {
        public sealed class MemberInfo
        {
            public uint PlayerId { get; set; }
        }

        public NoNullList<MemberInfo> Members { get; } = new NoNullList<MemberInfo>();

        public string ChannelName { get; set; }

        public ushort Password { get; set; }
    }
}
