using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_START_COOLTIME_SKILL")]
    public sealed class SStartCoolTimeSkillPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }

        public uint Cooldown { get; set; }
    }
}
