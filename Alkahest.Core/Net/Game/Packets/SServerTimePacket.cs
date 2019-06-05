using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SERVER_TIME")]
    public sealed class SServerTimePacket : SerializablePacket
    {
        public ulong ServerTime { get; set; }
    }
}
