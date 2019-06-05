using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_START_INSTANCE_SKILL")]
    public sealed class CStartInstanceSkillPacket : SerializablePacket
    {
        public sealed class TargetInfo
        {
            public uint ProjectileId { get; set; }

            public GameId Target { get; set; }

            public uint HitCylinderListIndex { get; set; }
        }

        public List<TargetInfo> Targets { get; } = new List<TargetInfo>();

        public sealed class EndPointInfo
        {
            public Vector3 Location { get; set; }
        }

        public List<EndPointInfo> EndPoints { get; } = new List<EndPointInfo>();

        public SkillId Skill { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public bool IsContinuation { get; set; }
    }
}
