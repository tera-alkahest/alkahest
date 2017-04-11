using System.Collections.Generic;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SInventoryPacket : Packet
    {
        const string Name = "S_INVEN";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SInventoryPacket();
        }

        public sealed class ItemInfo
        {
            public sealed class PassivityInfo
            {
                [PacketField]
                public uint PassivityId { get; set; }
            }

            [PacketField]
            public List<PassivityInfo> Passivities { get; } =
                new List<PassivityInfo>();

            [PacketField]
            public string Unknown1 { get; set; }

            [PacketField]
            public uint ItemId { get; set; }

            [PacketField]
            public EntityId Item { get; set; }

            [PacketField]
            public EntityId Owner1 { get; set; }

            [PacketField]
            public uint Slot { get; set; }

            [PacketField]
            public uint Unknown2 { get; set; }

            [PacketField]
            public uint Amount { get; set; }

            [PacketField]
            public uint EnchantmentLevel { get; set; }

            [PacketField]
            public uint Unknown3 { get; set; }

            [PacketField]
            public byte Unknown4 { get; set; }

            [PacketField]
            public uint Crystal1ItemId { get; set; }

            [PacketField]
            public uint Crystal2ItemId { get; set; }

            [PacketField]
            public uint Crystal3ItemId { get; set; }

            [PacketField]
            public uint Crystal4ItemId { get; set; }

            [PacketField]
            public uint Unknown5 { get; set; }

            [PacketField]
            public uint Unknown6 { get; set; }

            [PacketField]
            public uint Unknown7 { get; set; }

            [PacketField]
            public int Unknown8 { get; set; }

            [PacketField]
            public uint Unknown9 { get; set; }

            [PacketField]
            public uint Unknown10 { get; set; }

            [PacketField]
            public uint Unknown11 { get; set; }

            [PacketField]
            public uint Unknown12 { get; set; }

            [PacketField]
            public bool IsMasterworked { get; set; }

            [PacketField]
            public uint Unknown13 { get; set; }

            [PacketField]
            public uint Unknown14 { get; set; }

            [PacketField]
            public uint ItemLevel { get; set; }

            [PacketField]
            public uint MinItemLevel { get; set; }

            [PacketField]
            public uint MaxItemLevel { get; set; }

            [PacketField]
            public uint Unknown15 { get; set; }

            [PacketField]
            public uint Unknown16 { get; set; }

            [PacketField]
            public uint Unknown17 { get; set; }

            [PacketField]
            public uint Unknown18 { get; set; }

            [PacketField]
            public EntityId Owner2 { get; set; }

            [PacketField]
            public bool IsAwakened { get; set; }

            [PacketField]
            public uint Unknown19 { get; set; }

            [PacketField]
            public int Feedstock { get; set; }

            [PacketField]
            public uint Unknown20 { get; set; }

            [PacketField]
            public bool IsEquipped { get; set; }

            [PacketField]
            public byte Unknown21 { get; set; }
        }

        [PacketField]
        public List<ItemInfo> Items { get; } = new List<ItemInfo>();

        [PacketField]
        public EntityId Player { get; set; }

        [PacketField]
        public ulong Money { get; set; }

        [PacketField]
        public byte Unknown25 { get; set; }

        [PacketField]
        public byte Unknown26 { get; set; }

        [PacketField]
        public byte IsIncomplete { get; set; }

        [PacketField]
        public uint InventorySize { get; set; }

        [PacketField]
        public uint InventoryItemLevel { get; set; }

        [PacketField]
        public uint CharacterItemLevel { get; set; }

        [PacketField]
        public uint Unknown28 { get; set; }

        [PacketField]
        public uint Unknown29 { get; set; }

        [PacketField]
        public uint Unknown30 { get; set; }

        [PacketField]
        public uint Unknown31 { get; set; }

        [PacketField]
        public uint Unknown32 { get; set; }
    }
}
