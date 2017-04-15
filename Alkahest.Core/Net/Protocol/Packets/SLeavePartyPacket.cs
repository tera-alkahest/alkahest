namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SLeavePartyPacket : Packet
    {
        const string Name = "S_LEAVE_PARTY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLeavePartyPacket();
        }
    }
}
