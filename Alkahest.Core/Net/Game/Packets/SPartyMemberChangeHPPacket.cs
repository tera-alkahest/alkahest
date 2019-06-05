using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPartyMemberChangeHPPacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_CHANGE_HP";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberChangeHPPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public ulong CurrentHP { get; set; }

        [PacketField]
        public ulong MaxHP { get; set; }
    }
}
