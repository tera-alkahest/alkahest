using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ACTION_STAGE")]
    public sealed class SActionStagePacket : SerializablePacket
    {
        public sealed class AnimationSequenceInfo
        {
            public uint Duration { get; set; }

            public float XYSpeed { get; set; }

            public float ZSpeed { get; set; }

            public float Distance { get; set; }
        }

        public NoNullList<AnimationSequenceInfo> AnimationSequences { get; } =
            new NoNullList<AnimationSequenceInfo>();

        public GameId Source { get; set; }

        public Vector3 Position { get; set; }

        public Angle Angle { get; set; }

        public TemplateId Template { get; set; }

        public SkillId Skill { get; set; }

        public uint StageListIndex { get; set; }

        public float Speed { get; set; }

        public float ProjectileSpeed { get; set; }

        public uint CorrelationId { get; set; }

        public float EffectScale { get; set; }

        public bool IsMoving { get; set; }

        public Vector3 Destination { get; set; }

        public GameId Target { get; set; }
    }
}
