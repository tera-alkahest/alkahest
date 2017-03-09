namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CChatPacket : Packet
    {
        const string Name = "C_CHAT";

        public override string OpCode
        {
            get { return Name; }
        }

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CChatPacket();
        }

        [PacketField]
        public ushort MessageOffset { get; set; }

        [PacketField]
        public uint Channel { get; set; }

        [PacketField]
        public string Message { get; set; }
    }
}
