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

        public sealed class Movement
        {
            [PacketField]
            public uint Duration { get; set; }

            [PacketField]
            public float Speed { get; set; }

            [PacketField]
            public float Unknown5 { get; set; }

            [PacketField]
            public float Distance { get; set; }
        }

        [PacketField]
        public List<Movement> Movements { get; } = new List<Movement>();

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
        public uint Stage { get; set; }

        [PacketField]
        public float Speed { get; set; }

        [PacketField]
        public uint CorrelationId { get; set; }

        [PacketField]
        public float Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }
    }
}
