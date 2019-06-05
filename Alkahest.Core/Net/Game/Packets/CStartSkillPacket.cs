using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_START_SKILL")]
    public sealed class CStartSkillPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }

        public Angle Direction { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Destination { get; set; }

        public byte Unknown1 { get; set; }

        public bool IsMoving { get; set; }

        public bool IsContinuation { get; set; }

        public GameId Target { get; set; }

        public byte Unknown2 { get; set; }
    }
}
