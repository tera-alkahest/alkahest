using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_AVAILABLE_EVENT_MATCHING_LIST")]
    public sealed class CAvailableEventMatchingListPacket : SerializablePacket
    {
        public byte Unknown1 { get; set; }
    }
}
