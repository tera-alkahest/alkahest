using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_REQUEST_PRIVATE_CHANNEL_INFO")]
    public sealed class SRequestPrivateChannelInfoPacket : SerializablePacket
    {
        public sealed class FriendInfo
        {
            public string FriendName { get; set; }

            public uint PlayerId { get; set; }

            public Class Class { get; set; }

            public uint Level { get; set; }

            public uint FriendGroupId { get; set; }
        }

        public NoNullList<FriendInfo> Friends { get; } = new NoNullList<FriendInfo>();

        public sealed class MemberInfo
        {
            public string MemberName { get; set; }
        }

        public NoNullList<MemberInfo> Members { get; } = new NoNullList<MemberInfo>();

        public bool IsOwner { get; set; }

        public ushort Password { get; set; }
    }
}
