namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CCrestApplyPacket : Packet
    {
        const string Name = "C_CREST_APPLY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCrestApplyPacket();
        }

        [PacketField]
        public uint CrestId { get; set; }

        [PacketField]
        public bool IsActive { get; set; }
    }
}
