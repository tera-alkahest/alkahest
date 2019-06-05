using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SActionStagePacket : Packet
    {
        const string Name = "S_ACTION_STAGE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SActionStagePacket();
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
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Angle { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public uint StageListIndex { get; set; }

        [PacketField]
        public float Speed { get; set; }

        [PacketField]
        public float ProjectileSpeed { get; set; }

        [PacketField]
        public uint CorrelationId { get; set; }

        [PacketField]
        public float EffectScale { get; set; }

        [PacketField]
        public bool IsMoving { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public GameId Target { get; set; }
    }
}
