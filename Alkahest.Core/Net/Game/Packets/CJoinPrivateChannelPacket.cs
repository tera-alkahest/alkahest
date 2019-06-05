using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_JOIN_PRIVATE_CHANNEL")]
    public sealed class CJoinPrivateChannelPacket : SerializablePacket
    {
        public string ChannelName { get; set; }

        public ushort Password { get; set; }
    }
}
