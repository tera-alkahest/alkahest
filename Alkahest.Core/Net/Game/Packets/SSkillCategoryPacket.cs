using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SSkillCategoryPacket : Packet
    {
        const string Name = "S_SKILL_CATEGORY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSkillCategoryPacket();
        }

        [PacketField]
        public uint CategoryId { get; set; }

        [PacketField]
        public bool IsEnabled { get; set; }
    }
}
