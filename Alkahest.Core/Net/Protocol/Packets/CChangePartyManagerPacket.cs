namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CChangePartyManagerPacket : Packet
    {
        const string Name = "C_CHANGE_PARTY_MANAGER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CChangePartyManagerPacket();
        }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }
    }
}
