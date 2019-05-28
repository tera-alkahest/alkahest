namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLogOutPartyMemberPacket : Packet
    {
        const string Name = "S_LOGOUT_PARTY_MEMBER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLogOutPartyMemberPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }
    }
}
