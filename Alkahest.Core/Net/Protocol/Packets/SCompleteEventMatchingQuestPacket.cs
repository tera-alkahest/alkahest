namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SCompleteEventMatchingQuestPacket : Packet
    {
        const string Name = "S_COMPLETE_EVENT_MATCHING_QUEST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCompleteEventMatchingQuestPacket();
        }

        [PacketField]
        public int Id { get; set; }
    }
}
