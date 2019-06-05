using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_LEAVE_PRIVATE_CHANNEL")]
    public sealed class CLeavePrivateChannelPacket : SerializablePacket
    {
        public ushort Index { get; set; }
    }
}
