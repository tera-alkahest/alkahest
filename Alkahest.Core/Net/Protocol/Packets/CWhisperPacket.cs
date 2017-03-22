namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CWhisperPacket : Packet
    {
        const string Name = "C_WHISPER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CWhisperPacket();
        }

        [PacketField]
        internal ushort RecipientNameOffset { get; set; }

        [PacketField]
        internal ushort MessageOffset { get; set; }

        [PacketField]
        public string RecipientName { get; set; }

        [PacketField]
        public string Message { get; set; }
    }
}
