namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SClearQuestInfoPacket : Packet
    {
        const string Name = "S_CLEAR_QUEST_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SClearQuestInfoPacket();
        }
    }
}
