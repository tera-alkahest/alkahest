using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
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
            public sealed class PassivitySetInfo
            {
                public sealed class PassivityInfo
                {
                    [PacketField]
                    public uint PassivityId { get; set; }
                }

                [PacketField]
                public List<PassivityInfo> Passivities { get; } = new List<PassivityInfo>();

                [PacketField]
                public uint Index { get; set; }

                [PacketField]
                public uint MasterworkBonus { get; set; }

                [PacketField]
                public uint ItemLevel { get; set; }

                [PacketField]
                public uint MinItemLevel { get; set; }

                [PacketField]
                public uint MaxItemLevel { get; set; }
            }

            [PacketField]
            public List<PassivitySetInfo> PassivitySets { get; } = new List<PassivitySetInfo>();

            public sealed class MergedPassivityInfo
            {
                [PacketField]
                public uint PassivityId { get; set; }
            }

            [PacketField]
            public List<MergedPassivityInfo> MergedPassivities { get; } = new List<MergedPassivityInfo>();

            [PacketField]
            public string CustomString { get; set; }

            [PacketField]
            public uint ItemId { get; set; }

            [PacketField]
            public GameId Item { get; set; }

            [PacketField]
            public GameId Holder { get; set; }

            [PacketField]
            public uint Slot { get; set; }

            [PacketField]
            public int Unknown1 { get; set; }

            [PacketField]
            public uint Amount { get; set; }

            [PacketField]
            public uint EnchantmentLevel { get; set; }

            [PacketField]
            public uint Durability { get; set; }

            [PacketField]
            public bool IsSoulbound { get; set; }

            [PacketField]
            public uint Crystal1ItemId { get; set; }

            [PacketField]
            public uint Crystal2ItemId { get; set; }

            [PacketField]
            public uint Crystal3ItemId { get; set; }

            [PacketField]
            public uint Crystal4ItemId { get; set; }

            [PacketField]
            public int Unknown2 { get; set; }

            [PacketField]
            public uint PassivityInfoSetCount { get; set; }

            [PacketField]
            public uint ExtraPassivityInfoSetCount { get; set; }

            [PacketField]
            public int Unknown3 { get; set; }

            [PacketField]
            public int Unknown4 { get; set; }

            [PacketField]
            public int Unknown5 { get; set; }

            [PacketField]
            public long Unknown6 { get; set; }

            [PacketField]
            public long Unknown7 { get; set; }

            [PacketField]
            public bool IsMasterworked { get; set; }

            [PacketField]
            public int Unknown8 { get; set; }

            [PacketField]
            public uint TimesBrokered { get; set; }

            [PacketField]
            public uint EnchantmentAdvantage { get; set; }

            [PacketField]
            public uint EnchantmentBonus { get; set; }

            [PacketField]
            public int Unknown9 { get; set; }

            [PacketField]
            public GameId Owner { get; set; }

            [PacketField]
            public bool IsAwakened { get; set; }

            [PacketField]
            public int Unknown10 { get; set; }

            [PacketField]
            public int Unknown11 { get; set; }

            [PacketField]
            public int Unknown12 { get; set; }

            [PacketField]
            public bool HasEtching { get; set; }

            [PacketField]
            public byte Unknown13 { get; set; }

            [PacketField]
            public ulong ItemExperience { get; set; }

            [PacketField]
            public bool IsDamaged { get; set; }
        }

        [PacketField]
        public List<ItemInfo> Items { get; } = new List<ItemInfo>();

        [PacketField]
        public GameId Player { get; set; }

        [PacketField]
        public ulong Money { get; set; }

        [PacketField]
        public byte Unknown25 { get; set; }

        [PacketField]
        public bool IsFirst { get; set; }

        [PacketField]
        public bool IsIncomplete { get; set; }

        [PacketField]
        public uint InventorySize { get; set; }

        [PacketField]
        public uint InventoryItemLevel { get; set; }

        [PacketField]
        public uint CharacterItemLevel { get; set; }

        [PacketField]
        public long Unknown28 { get; set; }

        [PacketField]
        public int Unknown30 { get; set; }

        [PacketField]
        public long Unknown31 { get; set; }
    }
}
