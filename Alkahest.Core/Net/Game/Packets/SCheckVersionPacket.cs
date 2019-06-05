using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CHECK_VERSION")]
    public sealed class SCheckVersionPacket : SerializablePacket
    {
        public bool IsCompatible { get; set; }
    }
}
