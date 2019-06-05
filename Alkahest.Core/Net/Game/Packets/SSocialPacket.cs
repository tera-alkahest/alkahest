using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SOCIAL")]
    public sealed class SSocialPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public uint SocialId { get; set; }

        public int Unknown1 { get; set; }

        public byte Unknown2 { get; set; }
    }
}
