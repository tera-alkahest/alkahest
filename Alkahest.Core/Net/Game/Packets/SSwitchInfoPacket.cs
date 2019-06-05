using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SWITCH_INFO")]
    public sealed class SSwitchInfoPacket : SerializablePacket
    {
        public SkillId OnSkill { get; set; }

        public SkillId OffSkill { get; set; }

        public bool IsActive { get; set; }
    }
}
