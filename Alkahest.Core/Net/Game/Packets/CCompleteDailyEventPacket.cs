using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_COMPLETE_DAILY_EVENT")]
    public sealed class CCompleteDailyEventPacket : SerializablePacket
    {
        public uint QuestId { get; set; }
    }
}
