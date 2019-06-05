using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_REQUEST_MEGAPHONE")]
    public sealed class CRequestMegaphonePacket : SerializablePacket
    {
        public string Message { get; set; }

        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }
    }
}
