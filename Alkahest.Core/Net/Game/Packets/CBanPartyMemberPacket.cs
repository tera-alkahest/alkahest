namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CBanPartyMemberPacket : Packet
    {
        const string Name = "C_BAN_PARTY_MEMBER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CBanPartyMemberPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }
    }
}
