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

        public sealed class Movement
        {
            [PacketField]
            public uint Duration { get; set; }

            [PacketField]
            public float Speed { get; set; }

            [PacketField]
            public float Unknown1 { get; set; }

            [PacketField]
            public float Distance { get; set; }
        }

        [PacketField]
        public List<Movement> Movements { get; } = new List<Movement>();

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
        public uint Stage { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public uint CorrelationId { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Damage { get; set; }

        [PacketField]
        public ushort Unknown4 { get; set; }

        [PacketField]
        public ushort Unknown5 { get; set; }

        [PacketField]
        public bool IsCritical { get; set; }

        [PacketField]
        public byte Unknown6 { get; set; }

        [PacketField]
        public bool IsBlocked { get; set; }

        [PacketField]
        public byte Unknown7 { get; set; }

        [PacketField]
        public byte Unknown8 { get; set; }

        [PacketField]
        public byte Unknown9 { get; set; }

        [PacketField]
        public byte Unknown10 { get; set; }

        [PacketField]
        public bool SetTargetAction { get; set; }

        [PacketField]
        public byte Unknown11 { get; set; }

        [PacketField]
        public byte Unknown12 { get; set; }

        [PacketField]
        public byte Unknown13 { get; set; }

        [PacketField]
        public Vector3 TargetPosition { get; set; }

        [PacketField]
        public Angle TargetDirection { get; set; }

        [PacketField]
        public SkillId TargetSkill { get; set; }

        [PacketField]
        public uint TargetStage { get; set; }

        [PacketField]
        public uint TargetCorrelationId { get; set; }
    }
}
