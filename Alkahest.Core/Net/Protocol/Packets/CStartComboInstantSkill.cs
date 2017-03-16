using System.Numerics;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
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
