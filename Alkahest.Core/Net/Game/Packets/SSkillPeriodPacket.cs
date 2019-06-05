using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

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

        public List<SkillPeriodInfo> SkillPeriods { get; } = new List<SkillPeriodInfo>();

        public GameId Target { get; set; }
    }
}
