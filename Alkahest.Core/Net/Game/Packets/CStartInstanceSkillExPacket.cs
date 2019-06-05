using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_START_INSTANCE_SKILL_EX")]
    public sealed class CStartInstanceSkillExPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }

        public GameId Target { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Destination { get; set; }

        public Angle Direction { get; set; }

        public byte Unknown1 { get; set; }
    }
}
