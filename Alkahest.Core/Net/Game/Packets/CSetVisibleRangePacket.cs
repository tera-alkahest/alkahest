namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CSetVisibleRangePacket : Packet
    {
        const string Name = "C_SET_VISIBLE_RANGE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CSetVisibleRangePacket();
        }

        [PacketField]
        public uint Range { get; set; }
    }
}
