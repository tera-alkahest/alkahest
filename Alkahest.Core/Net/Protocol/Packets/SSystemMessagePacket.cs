namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SSystemMessagePacket : Packet
    {
        const string Name = "S_SYSTEM_MESSAGE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSystemMessagePacket();
        }

        [PacketField]
        internal ushort MessageOffset { get; set; }

        [PacketField]
        public string Message { get; set; }
    }
}
