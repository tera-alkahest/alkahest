using System.Collections.Generic;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SAddAwakenEnchantDataPacket : Packet
    {
        const string Name = "S_ADD_AWAKEN_ENCHANT_DATA";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAddAwakenEnchantDataPacket();
        }

        public sealed class EnchamentCostInfo
        {
            [PacketField]
            public uint Index { get; set; }

            [PacketField]
            public uint EnchantmentLevel { get; set; }

            [PacketField]
            public uint Tier { get; set; }

            [PacketField]
            public AwakenedEquipmentKind EquipmentKind { get; set; }

            [PacketField]
            public uint AlkahestItemId { get; set; }

            [PacketField]
            public uint AlkahestAmount { get; set; }

            [PacketField]
            public uint FeedstockAmount { get; set; }
        }

        [PacketField]
        public List<EnchamentCostInfo> EnchantmentCosts { get; } =
            new List<EnchamentCostInfo>();
    }
}
