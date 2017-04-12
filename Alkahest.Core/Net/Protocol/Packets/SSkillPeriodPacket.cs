using System.Collections.Generic;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
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
            public uint Unknown3 { get; set; }

            [PacketField]
            public uint Unknown4 { get; set; }
        }

        [PacketField]
        public List<SkillPeriodInfo> SkillPeriods { get; } =
            new List<SkillPeriodInfo>();

        [PacketField]
        public EntityId Target { get; set; }
    }
}
