using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_REQUEST_PRIVATE_CHANNEL_INFO")]
    public sealed class CRequestPrivateChannelInfoPacket : SerializablePacket
    {
        public uint ChannelId { get; set; }
    }
}
