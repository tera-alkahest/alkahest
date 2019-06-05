using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_WHISPER")]
    public sealed class CWhisperPacket : SerializablePacket
    {
        public string RecipientName { get; set; }

        public string Message { get; set; }
    }
}
