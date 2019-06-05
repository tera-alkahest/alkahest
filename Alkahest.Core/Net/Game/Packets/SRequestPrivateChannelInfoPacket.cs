using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SRequestPrivateChannelInfoPacket : Packet
    {
        const string Name = "S_REQUEST_PRIVATE_CHANNEL_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SRequestPrivateChannelInfoPacket();
        }

        public sealed class FriendInfo
        {
            [PacketField]
            public string FriendName { get; set; }

            [PacketField]
            public uint PlayerId { get; set; }

            [PacketField]
            public Class Class { get; set; }

            [PacketField]
            public uint Level { get; set; }

            [PacketField]
            public uint FriendGroupId { get; set; }
        }

        [PacketField]
        public List<FriendInfo> Friends { get; } = new List<FriendInfo>();

        public sealed class MemberInfo
        {
            [PacketField]
            public string MemberName { get; set; }
        }

        [PacketField]
        public List<MemberInfo> Members { get; } = new List<MemberInfo>();

        [PacketField]
        public bool IsOwner { get; set; }

        [PacketField]
        public ushort Password { get; set; }
    }
}
