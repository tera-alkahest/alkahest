using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SEachSkillResultPacket : Packet
    {
        const string Name = "S_EACH_SKILL_RESULT";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SEachSkillResultPacket();
        }

        public sealed class AnimationSequenceInfo
        {
            [PacketField]
            public uint Duration { get; set; }

            [PacketField]
            public float XYSpeed { get; set; }

            [PacketField]
            public float ZSpeed { get; set; }

            [PacketField]
            public float Distance { get; set; }
        }

        [PacketField]
        public List<AnimationSequenceInfo> AnimationSequences { get; } = new List<AnimationSequenceInfo>();

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public GameId ProjectileOwner { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public uint TargetingListIndex { get; set; }

        [PacketField]
        public uint AreaListIndex { get; set; }

        [PacketField]
        public uint CorrelationId { get; set; }

        [PacketField]
        public uint TargetingTime { get; set; }

        [PacketField]
        public ulong Damage { get; set; }

        [PacketField]
        public SkillResultKind Kind { get; set; }

        [PacketField]
        public ushort NocteniumEffectType { get; set; }

        [PacketField]
        public bool IsCritical { get; set; }

        [PacketField]
        public bool ConsumeStacks { get; set; }

        [PacketField]
        public bool IsSuperArmor { get; set; }

        [PacketField]
        public uint SuperArmorEffectId { get; set; }

        [PacketField]
        public uint HitCylinderListIndex { get; set; }

        [PacketField]
        public bool IsReaction { get; set; }

        [PacketField]
        public bool IsPushback { get; set; }

        [PacketField]
        public byte IsKnockUp { get; set; }

        [PacketField]
        public byte IsKnockUpChain { get; set; }

        [PacketField]
        public Vector3 TargetPosition { get; set; }

        [PacketField]
        public Angle TargetDirection { get; set; }

        [PacketField]
        public SkillId TargetSkill { get; set; }

        [PacketField]
        public uint TargetStageListIndex { get; set; }

        [PacketField]
        public uint TargetCorrelationId { get; set; }
    }
}
