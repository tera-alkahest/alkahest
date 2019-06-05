using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LEAVE_PRIVATE_CHANNEL")]
    public sealed class SLeavePrivateChannelPacket : SerializablePacket
    {
        public uint ChannelId { get; set; }
    }
}
