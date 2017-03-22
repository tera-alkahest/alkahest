namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CRequestMegaphonePacket : Packet
    {
        const string Name = "C_REQUEST_MEGAPHONE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CRequestMegaphonePacket();
        }

        [PacketField]
        internal ushort MessageOffset { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public string Message { get; set; }
    }
}
