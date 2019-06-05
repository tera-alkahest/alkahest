using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SPAWN_NPC")]
    public sealed class SSpawnNpcPacket : SerializablePacket
    {
        public sealed class SeatInfo
        {
            public uint SeatId { get; set; }

            public GameId Passenger { get; set; }

            public Angle Pitch { get; set; }

            public Angle Yaw { get; set; }

            public ulong MaxHP { get; set; }

            public uint MaxMP { get; set; }

            public ulong CurrentHP { get; set; }

            public uint CurrentMP { get; set; }
        }

        public List<SeatInfo> Seats { get; set; } = new List<SeatInfo>();

        public sealed class PartInfo
        {
            public uint PartId { get; set; }

            public uint CurrentBreakStageId { get; set; }

            public bool IsActive { get; set; }

            public uint LastBreakStageId { get; set; }
        }

        public List<PartInfo> Parts { get; set; } = new List<PartInfo>();

        public string NpcName { get; set; }

        public GameId Npc { get; set; }

        public GameId Target { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public int Unknown1 { get; set; }

        public TemplateId Template { get; set; }

        public ushort HuntingZoneId { get; set; }

        public int Unknown2 { get; set; }

        public ushort WalkSpeed { get; set; }

        public ushort RunSpeed { get; set; }

        public NpcStatus Status { get; set; }

        public bool IsEnraged { get; set; }

        public byte Unknown3 { get; set; }

        public uint EnrageTimeRemaining { get; set; }

        public HPLevel HPLevel { get; set; }

        public short Unknown4 { get; set; }

        public bool IsVisible { get; set; }

        public bool IsVillager { get; set; }

        public int Unknown5 { get; set; }

        public GameId Replaced { get; set; }

        public uint SpawnScriptId { get; set; }

        public uint ReplaceScriptId { get; set; }

        public bool IsAggressive { get; set; }

        public GameId Owner { get; set; }

        public GameId Unknown6 { get; set; }

        public GameId Unknown7 { get; set; }

        public byte Unknown8 { get; set; }

        public byte Unknown9 { get; set; }

        public short Unknown10 { get; set; }

        public uint HitCylinderListIndex { get; set; }

        public byte Unknown11 { get; set; }

        public int Unknown12 { get; set; }

        public byte Unknown13 { get; set; }
    }
}
