using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
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

        [PacketField]
        internal ushort FriendsCount { get; set; }

        [PacketField]
        internal ushort FriendsOffset { get; set; }

        [PacketField]
        internal ushort MembersCount { get; set; }

        [PacketField]
        internal ushort MembersOffset { get; set; }

        [PacketField]
        public bool IsOwner { get; set; }

        [PacketField]
        public ushort Password { get; set; }

        public sealed class Member
        {
            [PacketField]
            internal ushort MemberNameOffset { get; set; }

            [PacketField]
            public string MemberName { get; set; }
        }

        [PacketField]
        public Member[] Members { get; set; }

        public sealed class Friend
        {
            [PacketField]
            internal ushort FriendNameOffset { get; set; }

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
        public Friend[] Friends { get; set; }
    }
}
