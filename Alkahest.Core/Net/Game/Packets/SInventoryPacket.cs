using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_INVEN")]
    public sealed class SInventoryPacket : SerializablePacket
    {
        public sealed class ItemInfo
        {
            public sealed class PassivitySetInfo
            {
                public sealed class PassivityInfo
                {
                    public uint PassivityId { get; set; }
                }

                public NoNullList<PassivityInfo> Passivities { get; } =
                    new NoNullList<PassivityInfo>();

                public uint Index { get; set; }

                public uint MasterworkBonus { get; set; }

                public uint ItemLevel { get; set; }

                public uint MinItemLevel { get; set; }

                public uint MaxItemLevel { get; set; }
            }

            public NoNullList<PassivitySetInfo> PassivitySets { get; } =
                new NoNullList<PassivitySetInfo>();

            public sealed class MergedPassivityInfo
            {
                public uint PassivityId { get; set; }
            }

            public NoNullList<MergedPassivityInfo> MergedPassivities { get; } =
                new NoNullList<MergedPassivityInfo>();

            public string CustomString { get; set; }

            public uint ItemId { get; set; }

            public GameId Item { get; set; }

            public GameId Holder { get; set; }

            public uint Slot { get; set; }

            public int Unknown1 { get; set; }

            public uint Amount { get; set; }

            public uint EnchantmentLevel { get; set; }

            public uint Durability { get; set; }

            public bool IsSoulbound { get; set; }

            public uint Crystal1ItemId { get; set; }

            public uint Crystal2ItemId { get; set; }

            public uint Crystal3ItemId { get; set; }

            public uint Crystal4ItemId { get; set; }

            public int Unknown2 { get; set; }

            public uint PassivityInfoSetCount { get; set; }

            public uint ExtraPassivityInfoSetCount { get; set; }

            public int Unknown3 { get; set; }

            public int Unknown4 { get; set; }

            public int Unknown5 { get; set; }

            public long Unknown6 { get; set; }

            public long Unknown7 { get; set; }

            public bool IsMasterworked { get; set; }

            public int Unknown8 { get; set; }

            public uint TimesBrokered { get; set; }

            public uint EnchantmentAdvantage { get; set; }

            public uint EnchantmentBonus { get; set; }

            public int Unknown9 { get; set; }

            public GameId Owner { get; set; }

            public bool IsAwakened { get; set; }

            public int Unknown10 { get; set; }

            public int Unknown11 { get; set; }

            public int Unknown12 { get; set; }

            public bool HasEtching { get; set; }

            public byte Unknown13 { get; set; }

            public ulong ItemExperience { get; set; }

            public bool IsDamaged { get; set; }
        }

        public NoNullList<ItemInfo> Items { get; } = new NoNullList<ItemInfo>();

        public GameId Player { get; set; }

        public ulong Money { get; set; }

        public byte Unknown25 { get; set; }

        public bool IsFirst { get; set; }

        public bool IsIncomplete { get; set; }

        public uint InventorySize { get; set; }

        public uint InventoryItemLevel { get; set; }

        public uint CharacterItemLevel { get; set; }

        public long Unknown28 { get; set; }

        public int Unknown30 { get; set; }

        public long Unknown31 { get; set; }
    }
}
