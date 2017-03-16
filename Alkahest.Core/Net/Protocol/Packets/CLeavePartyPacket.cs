namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CLeavePartyPacket : Packet
    {
        const string Name = "C_LEAVE_PARTY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CLeavePartyPacket();
        }
    }
}
