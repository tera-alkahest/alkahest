using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_START_TARGETED_SKILL")]
    public sealed class CStartTargetedSkillPacket : SerializablePacket
    {
        public sealed class TargetInfo
        {
            public GameId Target { get; set; }

            public int Unknown1 { get; set; }
        }

        public NoNullList<TargetInfo> Targets { get; } = new NoNullList<TargetInfo>();

        public SkillId Skill { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public Vector3 Destination { get; set; }
    }
}
