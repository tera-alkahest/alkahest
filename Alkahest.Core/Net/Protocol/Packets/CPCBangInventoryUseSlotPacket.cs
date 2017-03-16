namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CPCBangInventoryUseSlotPacket : Packet
    {
        const string Name = "C_PCBANGINVENTORY_USE_SLOT";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CPCBangInventoryUseSlotPacket();
        }

        [PacketField]
        public uint Slot { get; set; }
    }
}
