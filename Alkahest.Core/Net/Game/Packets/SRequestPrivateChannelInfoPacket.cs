using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

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

        public List<FriendInfo> Friends { get; } = new List<FriendInfo>();

        public sealed class MemberInfo
        {
            public string MemberName { get; set; }
        }

        public List<MemberInfo> Members { get; } = new List<MemberInfo>();

        public bool IsOwner { get; set; }

        public ushort Password { get; set; }
    }
}
