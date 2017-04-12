using System.Collections.Generic;
using Alkahest.Core.Game;

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

        public sealed class Friend
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
        public List<Friend> Friends { get; } = new List<Friend>();

        public sealed class Member
        {
            [PacketField]
            public string MemberName { get; set; }
        }

        [PacketField]
        public List<Member> Members { get; } = new List<Member>();

        [PacketField]
        public bool IsOwner { get; set; }

        [PacketField]
        public ushort Password { get; set; }
    }
}
