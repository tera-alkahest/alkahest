using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SSkillPeriodPacket : Packet
    {
        const string Name = "S_SKILL_PERIOD";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSkillPeriodPacket();
        }

        public sealed class SkillPeriodInfo
        {
            [PacketField]
            public SkillId Skill { get; set; }

            [PacketField]
            public uint Time { get; set; }

            [PacketField]
            public int Unknown1 { get; set; }

            [PacketField]
            public int Unknown2 { get; set; }
        }

        [PacketField]
        public List<SkillPeriodInfo> SkillPeriods { get; } = new List<SkillPeriodInfo>();

        [PacketField]
        public GameId Target { get; set; }
    }
}
