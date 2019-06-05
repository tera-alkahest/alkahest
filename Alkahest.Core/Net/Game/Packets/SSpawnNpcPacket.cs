using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SSpawnNpcPacket : Packet
    {
        const string Name = "S_SPAWN_NPC";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSpawnNpcPacket();
        }

        public sealed class SeatInfo
        {
            [PacketField]
            public uint SeatId { get; set; }

            [PacketField]
            public GameId Passenger { get; set; }

            [PacketField]
            public Angle Pitch { get; set; }

            [PacketField]
            public Angle Yaw { get; set; }

            [PacketField]
            public ulong MaxHP { get; set; }

            [PacketField]
            public uint MaxMP { get; set; }

            [PacketField]
            public ulong CurrentHP { get; set; }

            [PacketField]
            public uint CurrentMP { get; set; }
        }

        [PacketField]
        public List<SeatInfo> Seats { get; set; } = new List<SeatInfo>();

        public sealed class PartInfo
        {
            [PacketField]
            public uint PartId { get; set; }

            [PacketField]
            public uint CurrentBreakStageId { get; set; }

            [PacketField]
            public bool IsActive { get; set; }

            [PacketField]
            public uint LastBreakStageId { get; set; }
        }

        [PacketField]
        public List<PartInfo> Parts { get; set; } = new List<PartInfo>();

        [PacketField]
        public string NpcName { get; set; }

        [PacketField]
        public GameId Npc { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public ushort HuntingZoneId { get; set; }

        [PacketField]
        public int Unknown2 { get; set; }

        [PacketField]
        public ushort WalkSpeed { get; set; }

        [PacketField]
        public ushort RunSpeed { get; set; }

        [PacketField]
        public NpcStatus Status { get; set; }

        [PacketField]
        public bool IsEnraged { get; set; }

        [PacketField]
        public byte Unknown3 { get; set; }

        [PacketField]
        public uint EnrageTimeRemaining { get; set; }

        [PacketField]
        public HPLevel HPLevel { get; set; }

        [PacketField]
        public short Unknown4 { get; set; }

        [PacketField]
        public bool IsVisible { get; set; }

        [PacketField]
        public bool IsVillager { get; set; }

        [PacketField]
        public int Unknown5 { get; set; }

        [PacketField]
        public GameId Replaced { get; set; }

        [PacketField]
        public uint SpawnScriptId { get; set; }

        [PacketField]
        public uint ReplaceScriptId { get; set; }

        [PacketField]
        public bool IsAggressive { get; set; }

        [PacketField]
        public GameId Owner { get; set; }

        [PacketField]
        public GameId Unknown6 { get; set; }

        [PacketField]
        public GameId Unknown7 { get; set; }

        [PacketField]
        public byte Unknown8 { get; set; }

        [PacketField]
        public byte Unknown9 { get; set; }

        [PacketField]
        public short Unknown10 { get; set; }

        [PacketField]
        public uint HitCylinderListIndex { get; set; }

        [PacketField]
        public byte Unknown11 { get; set; }

        [PacketField]
        public int Unknown12 { get; set; }

        [PacketField]
        public byte Unknown13 { get; set; }
    }
}
