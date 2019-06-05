using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
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
        public Vector3 Position { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }

        [PacketField]
        public bool IsMoving { get; set; }

        [PacketField]
        public bool IsContinuation { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
