using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CANCEL_SKILL")]
    public sealed class CCancelSkillPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }

        public uint Type { get; set; }
    }
}
