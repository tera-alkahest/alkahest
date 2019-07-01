using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SKILL_PERIOD")]
    public sealed class SSkillPeriodPacket : SerializablePacket
    {
        public sealed class SkillPeriodInfo
        {
            public SkillId Skill { get; set; }

            public uint Time { get; set; }

            public int Unknown1 { get; set; }

            public int Unknown2 { get; set; }
        }

        public NoNullList<SkillPeriodInfo> SkillPeriods { get; } =
            new NoNullList<SkillPeriodInfo>();

        public GameId Target { get; set; }
    }
}
