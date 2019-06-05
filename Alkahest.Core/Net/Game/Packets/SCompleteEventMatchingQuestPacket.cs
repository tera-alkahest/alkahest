using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_COMPLETE_EVENT_MATCHING_QUEST")]
    public sealed class SCompleteEventMatchingQuestPacket : SerializablePacket
    {
        public uint QuestId { get; set; }
    }
}
