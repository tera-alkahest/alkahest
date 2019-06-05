using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SKILL_CATEGORY")]
    public sealed class SSkillCategoryPacket : SerializablePacket
    {
        public uint CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
