using System.Numerics;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
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

        [PacketField]
        internal ushort MovementsCount { get; set; }

        [PacketField]
        internal ushort MovementsOffset { get; set; }

        [PacketField]
        public EntityId Source { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Angle { get; set; }

        [PacketField]
        public uint Model { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public uint Stage { get; set; }

        [PacketField]
        public float Speed { get; set; }

        [PacketField]
        public uint CorrelationId { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }

        public sealed class Movement
        {
            [PacketField]
            public uint Duration { get; set; }

            [PacketField]
            public float Speed { get; set; }

            [PacketField]
            public uint Unknown5 { get; set; }

            [PacketField]
            public float Distance { get; set; }
        }

        [PacketField]
        public Movement[] Movements { get; set; }
    }
}
