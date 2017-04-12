using System.Collections.Generic;
using System.Numerics;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CStartInstanceSkillPacket : Packet
    {
        const string Name = "C_START_INSTANCE_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CStartInstanceSkillPacket();
        }

        public sealed class TargetInfo
        {
            [PacketField]
            public uint Unknown2 { get; set; }

            [PacketField]
            public EntityId Target { get; set; }

            [PacketField]
            public uint Unknown3 { get; set; }
        }

        [PacketField]
        public List<TargetInfo> Targets { get; } = new List<TargetInfo>();

        public sealed class TargetLocation
        {
            [PacketField]
            public Vector3 Location { get; set; }
        }

        [PacketField]
        public List<TargetLocation> Locations { get; } =
            new List<TargetLocation>();

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
