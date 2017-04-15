namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SPartyMemberChangeMPPacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_CHANGE_MP";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberChangeMPPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public uint CurrentMP { get; set; }

        [PacketField]
        public uint MaxMP { get; set; }
    }
}
