using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CHECK_USERNAME")]
    public sealed class SCheckUserNamePacket : SerializablePacket
    {
        public bool IsAvailable { get; set; }
    }
}
