using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_SOCIAL")]
    public sealed class CSocialPacket : SerializablePacket
    {
        public uint SocialId { get; set; }

        public byte Unknown1 { get; set; }
    }
}
