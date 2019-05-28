namespace Alkahest.Core.Net.Game.Packets
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
        public string RecipientName { get; set; }

        [PacketField]
        public string Message { get; set; }
    }
}
