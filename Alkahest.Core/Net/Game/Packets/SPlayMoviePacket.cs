using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PLAY_MOVIE")]
    public sealed class SPlayMoviePacket : SerializablePacket
    {
        public uint MovieGroupId { get; set; }
    }
}
