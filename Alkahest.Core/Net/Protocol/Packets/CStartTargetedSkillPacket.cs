using System.Numerics;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
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

        [PacketField]
        internal ushort Unknown1Count { get; set; }

        [PacketField]
        internal ushort Unknown1Offset { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public Vector3[] Unknown1 { get; set; }
    }
}
