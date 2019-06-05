using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_END_MOVIE")]
    public sealed class CEndMoviePacket : SerializablePacket
    {
        public uint MovieGroupId { get; set; }

        public byte Unknown1 { get; set; }
    }
}
