using System.Collections.Generic;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SAddAwakenChangeDataPacket : Packet
    {
        const string Name = "S_ADD_AWAKEN_CHANGE_DATA";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAddAwakenChangeDataPacket();
        }

        public sealed class AwakeningCostInfo
        {
            [PacketField]
            public uint UniqueId { get; set; }

            [PacketField]
            public uint AlkahestItemId { get; set; }

            [PacketField]
            public uint AlkahestAmount { get; set; }

            [PacketField]
            public uint FeedstockAmount { get; set; }
        }

        [PacketField]
        public List<AwakeningCostInfo> AwakeningCosts { get; } =
            new List<AwakeningCostInfo>();
    }
}
