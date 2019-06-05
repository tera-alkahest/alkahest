using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CStartTargetedSkillPacket : Packet
    {
        const string Name = "C_START_TARGETED_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CStartTargetedSkillPacket();
        }

        public sealed class TargetInfo
        {
            [PacketField]
            public GameId Target { get; set; }

            [PacketField]
            public int Unknown1 { get; set; }
        }

        [PacketField]
        public List<TargetInfo> Targets { get; } = new List<TargetInfo>();

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }
    }
}
