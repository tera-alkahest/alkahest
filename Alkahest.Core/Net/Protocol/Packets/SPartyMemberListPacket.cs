using System.Collections.Generic;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SPartyMemberListPacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberListPacket();
        }

        public sealed class PartyMemberInfo
        {
            [PacketField]
            public string UserName { get; set; }

            [PacketField]
            public uint ServerId { get; set; }

            [PacketField]
            public uint PlayerId { get; set; }

            [PacketField]
            public uint Level { get; set; }

            [PacketField]
            public Class Class { get; set; }

            [PacketField]
            public bool IsOnline { get; set; }

            [PacketField]
            public EntityId Member { get; set; }

            [PacketField]
            public uint Position { get; set; }

            [PacketField]
            public byte Unknown1 { get; set; }

            [PacketField]
            public LaurelKind Laurel { get; set; }
        }

        [PacketField]
        public List<PartyMemberInfo> PartyMembers { get; } =
            new List<PartyMemberInfo>();

        [PacketField]
        public bool IsInstanceMatched { get; set; }

        [PacketField]
        public bool IsRaid { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }

        [PacketField]
        public ushort Unknown5 { get; set; }

        [PacketField]
        public ushort Unknown6 { get; set; }

        [PacketField]
        public uint LeaderServerId { get; set; }

        [PacketField]
        public uint LeaderPlayerId { get; set; }

        [PacketField]
        public uint Unknown7 { get; set; }

        [PacketField]
        public uint Unknown8 { get; set; }

        [PacketField]
        public byte Unknown9 { get; set; }

        [PacketField]
        public uint Unknown10 { get; set; }

        [PacketField]
        public byte Unknown11 { get; set; }

        [PacketField]
        public uint Unknown12 { get; set; }

        [PacketField]
        public byte Unknown13 { get; set; }
    }
}
