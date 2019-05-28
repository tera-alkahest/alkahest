using Alkahest.Core.Game;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CStartComboInstantSkillPacket : Packet
    {
        const string Name = "C_START_COMBO_INSTANT_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CStartComboInstantSkillPacket();
        }

        public sealed class TargetInfo
        {
            [PacketField]
            public uint Unknown1 { get; set; }

            [PacketField]
            public GameId Target { get; set; }

            [PacketField]
            public uint Unknown2 { get; set; }
        }

        [PacketField]
        public List<TargetInfo> Targets { get; } = new List<TargetInfo>();

        public sealed class TargetLocation
        {
            [PacketField]
            public Vector3 Location { get; set; }
        }

        [PacketField]
        public List<TargetLocation> Locations { get; } = new List<TargetLocation>();

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }
    }
}
