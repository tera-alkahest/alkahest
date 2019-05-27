using Alkahest.Core.Game;
using System.Numerics;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CStartSkillPacket : Packet
    {
        const string Name = "C_START_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CStartSkillPacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public Vector3 Position1 { get; set; }

        [PacketField]
        public Vector3 Position2 { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public byte Unknown3 { get; set; }

        [PacketField]
        public GameId Target { get; set; }
    }
}
