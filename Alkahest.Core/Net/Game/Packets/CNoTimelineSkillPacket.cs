using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_NOTIMELINE_SKILL")]
    public sealed class CNoTimelineSkillPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }
    }
}
