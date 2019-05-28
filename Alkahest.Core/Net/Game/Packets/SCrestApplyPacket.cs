namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SCrestApplyPacket : Packet
    {
        const string Name = "S_CREST_APPLY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCrestApplyPacket();
        }

        [PacketField]
        public uint CrestId { get; set; }

        [PacketField]
        public bool IsActive { get; set; }
    }
}
