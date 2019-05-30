using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
