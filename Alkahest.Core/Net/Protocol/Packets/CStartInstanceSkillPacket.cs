using System.Numerics;
using Alkahest.Core.Data;

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

        [PacketField]
        internal ushort TargetsCount { get; set; }

        [PacketField]
        internal ushort TargetsOffset { get; set; }

        [PacketField]
        internal ushort LocationsCount { get; set; }

        [PacketField]
        internal ushort LocationsOffset { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }

        public sealed class TargetInfo
        {
            [PacketField]
            public uint Unknown2 { get; set; }

            [PacketField]
            public EntityId Target { get; set; }
        }

        [PacketField]
        public TargetInfo[] Targets { get; set; }

        [PacketField]
        public Vector3[] Locations { get; set; }
    }
}
