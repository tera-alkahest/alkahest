using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_MEMBER_LIST")]
    public sealed class SPartyMemberListPacket : SerializablePacket
    {
        public sealed class PartyMemberInfo
        {
            public string UserName { get; set; }

            public uint ServerId { get; set; }

            public uint PlayerId { get; set; }

            public uint Level { get; set; }

            public Class Class { get; set; }

            public bool IsOnline { get; set; }

            public GameId Member { get; set; }

            public uint Order { get; set; }

            public byte Unknown1 { get; set; }

            public LaurelKind Laurel { get; set; }

            public uint ApexLevel { get; set; }
        }

        public NoNullList<PartyMemberInfo> PartyMembers { get; } =
            new NoNullList<PartyMemberInfo>();

        public bool IsInstanceMatched { get; set; }

        public bool IsRaid { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public short Unknown4 { get; set; }

        public short Unknown5 { get; set; }

        public uint LeaderServerId { get; set; }

        public uint LeaderPlayerId { get; set; }

        public int Unknown6 { get; set; }

        public int Unknown7 { get; set; }

        public byte Unknown8 { get; set; }

        public int Unknown9 { get; set; }

        public byte Unknown10 { get; set; }

        public int Unknown11 { get; set; }

        public byte Unknown12 { get; set; }

        public byte Unknown13 { get; set; }
    }
}
