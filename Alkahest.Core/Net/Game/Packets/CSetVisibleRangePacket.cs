using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_SET_VISIBLE_RANGE")]
    public sealed class CSetVisibleRangePacket : SerializablePacket
    {
        public uint Range { get; set; }
    }
}
