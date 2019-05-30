using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SClearWorldQuestVillagerInfoPacket : Packet
    {
        const string Name = "S_CLEAR_WORLD_QUEST_VILLAGER_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SClearWorldQuestVillagerInfoPacket();
        }
    }
}
