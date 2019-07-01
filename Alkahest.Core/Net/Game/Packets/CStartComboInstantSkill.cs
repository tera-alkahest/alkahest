using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_START_COMBO_INSTANT_SKILL")]
    public sealed class CStartComboInstantSkillPacket : SerializablePacket
    {
        public sealed class TargetInfo
        {
            public uint ProjectileId { get; set; }

            public GameId Target { get; set; }

            public uint HitCylinderListIndex { get; set; }
        }

        public NoNullList<TargetInfo> Targets { get; } = new NoNullList<TargetInfo>();

        public sealed class EndPointInfo
        {
            public Vector3 Position { get; set; }
        }

        public NoNullList<EndPointInfo> EndPoints { get; } = new NoNullList<EndPointInfo>();

        public SkillId Skill { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }
    }
}
