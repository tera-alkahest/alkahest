using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CANNOT_START_SKILL")]
    public sealed class CCannotStartSkillPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }
    }
}
