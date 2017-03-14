namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CSetVisibilityRangePacket : Packet
    {
        const string Name = "C_SET_VISIBLE_RANGE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CSetVisibilityRangePacket();
        }

        [PacketField]
        public uint Range { get; set; }
    }
}
