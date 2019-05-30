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

        public sealed class Unknown8Info
        {
            [PacketField]
            public uint Unknown1 { get; set; }

            [PacketField]
            public ulong Unknown2 { get; set; }

            [PacketField]
            public float Unknown3 { get; set; }

            [PacketField]
            public uint Unknown4 { get; set; }

            [PacketField]
            public uint Unknown5 { get; set; }

            [PacketField]
            public uint Unknown6 { get; set; }

            [PacketField]
            public uint Unknown7 { get; set; }
        }

        [PacketField]
        public List<Unknown8Info> Unknown8 { get; } = new List<Unknown8Info>();

        [PacketField(IsUnknownArray = true)]
        public ushort Unknown9Count { get; set; }

        [PacketField(IsUnknownArray = true)]
        public ushort Unknown9Offset { get; set; }

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
        public uint Unknown10 { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public uint HuntingZoneId { get; set; }

        [PacketField]
        public ushort Unknown12 { get; set; }

        [PacketField]
        public ushort Unknown13 { get; set; }

        [PacketField]
        public ushort Unknown14 { get; set; }

        [PacketField]
        public uint Unknown15 { get; set; }

        [PacketField]
        public byte Unknown16 { get; set; }

        [PacketField]
        public byte Unknown17 { get; set; }

        [PacketField]
        public uint Unknown18 { get; set; }

        [PacketField]
        public ulong Unknown19 { get; set; }

        [PacketField]
        public ushort Unknown20 { get; set; }

        [PacketField]
        public ushort Unknown21 { get; set; }

        [PacketField]
        public uint Unknown22 { get; set; }

        [PacketField]
        public byte Unknown23 { get; set; }

        [PacketField]
        public GameId Owner { get; set; }

        [PacketField]
        public uint Unknown24 { get; set; }

        [PacketField]
        public uint Unknown25 { get; set; }

        [PacketField]
        public ulong Unknown26 { get; set; }

        [PacketField]
        public byte Unknown27 { get; set; }

        [PacketField]
        public uint Unknown28 { get; set; }

        [PacketField]
        public uint Unknown29 { get; set; }
    }
}
