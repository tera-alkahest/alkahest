using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CREST_MESSAGE")]
    public sealed class SCrestMessagePacket : SerializablePacket
    {
        public uint CrestId { get; set; }

        public CrestMessageKind Kind { get; set; }

        [PacketFieldOptions(IsSimpleSkill = true)]
        public SkillId Skill { get; set; }
    }
}
