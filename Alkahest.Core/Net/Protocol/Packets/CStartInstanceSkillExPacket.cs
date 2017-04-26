using System.Numerics;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CStartInstanceSkillExPacket : Packet
    {
        const string Name = "C_START_INSTANCE_SKILL_EX";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CNoTimelinePacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
