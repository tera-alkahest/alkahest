using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_EACH_SKILL_RESULT")]
    public sealed class SEachSkillResultPacket : SerializablePacket
    {
        public sealed class AnimationSequenceInfo
        {
            public uint Duration { get; set; }

            public float XYSpeed { get; set; }

            public float ZSpeed { get; set; }

            public float Distance { get; set; }
        }

        public List<AnimationSequenceInfo> AnimationSequences { get; } = new List<AnimationSequenceInfo>();

        public GameId Source { get; set; }

        public GameId ProjectileOwner { get; set; }

        public GameId Target { get; set; }

        public TemplateId Template { get; set; }

        public SkillId Skill { get; set; }

        public uint TargetingListIndex { get; set; }

        public uint AreaListIndex { get; set; }

        public uint CorrelationId { get; set; }

        public uint TargetingTime { get; set; }

        public ulong Damage { get; set; }

        public SkillResultKind Kind { get; set; }

        public ushort NocteniumEffectType { get; set; }

        public bool IsCritical { get; set; }

        public bool ConsumeStacks { get; set; }

        public bool IsSuperArmor { get; set; }

        public uint SuperArmorEffectId { get; set; }

        public uint HitCylinderListIndex { get; set; }

        public bool IsReaction { get; set; }

        public bool IsPushback { get; set; }

        public byte IsKnockUp { get; set; }

        public byte IsKnockUpChain { get; set; }

        public Vector3 TargetPosition { get; set; }

        public Angle TargetDirection { get; set; }

        public SkillId TargetSkill { get; set; }

        public uint TargetStageListIndex { get; set; }

        public uint TargetCorrelationId { get; set; }
    }
}
