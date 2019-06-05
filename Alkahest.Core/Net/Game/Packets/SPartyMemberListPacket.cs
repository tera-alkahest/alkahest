using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
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
            public GameId Member { get; set; }

            [PacketField]
            public uint Order { get; set; }

            [PacketField]
            public byte Unknown1 { get; set; }

            [PacketField]
            public LaurelKind Laurel { get; set; }

            [PacketField]
            public uint ApexLevel { get; set; }
        }

        [PacketField]
        public List<PartyMemberInfo> PartyMembers { get; } = new List<PartyMemberInfo>();

        [PacketField]
        public bool IsInstanceMatched { get; set; }

        [PacketField]
        public bool IsRaid { get; set; }

        [PacketField]
        public int Unknown2 { get; set; }

        [PacketField]
        public int Unknown3 { get; set; }

        [PacketField]
        public short Unknown4 { get; set; }

        [PacketField]
        public short Unknown5 { get; set; }

        [PacketField]
        public uint LeaderServerId { get; set; }

        [PacketField]
        public uint LeaderPlayerId { get; set; }

        [PacketField]
        public int Unknown6 { get; set; }

        [PacketField]
        public int Unknown7 { get; set; }

        [PacketField]
        public byte Unknown8 { get; set; }

        [PacketField]
        public int Unknown9 { get; set; }

        [PacketField]
        public byte Unknown10 { get; set; }

        [PacketField]
        public int Unknown11 { get; set; }

        [PacketField]
        public byte Unknown12 { get; set; }

        [PacketField]
        public byte Unknown13 { get; set; }
    }
}
