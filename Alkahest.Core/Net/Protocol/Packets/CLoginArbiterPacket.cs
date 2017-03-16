namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CLoginArbiterPacket : Packet
    {
        const string Name = "C_LOGIN_ARBITER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CLoginArbiterPacket();
        }

        [PacketField]
        internal ushort AccountNameOffset { get; set; }

        [PacketField]
        internal ushort TicketOffset { get; set; }

        [PacketField]
        internal ushort TicketCount { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint PatchVersion { get; set; }

        [PacketField]
        public string AccountName { get; set; }

        [PacketField]
        public byte[] Ticket { get; set; }
    }
}
