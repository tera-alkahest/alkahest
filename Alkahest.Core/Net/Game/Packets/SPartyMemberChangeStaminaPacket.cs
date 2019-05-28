namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPartyMemberChangeStaminaPacket : Packet
    {
        const string Name = "S_PARTY_MEMBER_CHANGE_STAMINA";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyMemberChangeStaminaPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public uint CurrentResource { get; set; }

        [PacketField]
        public uint MaxResource { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }
    }
}
