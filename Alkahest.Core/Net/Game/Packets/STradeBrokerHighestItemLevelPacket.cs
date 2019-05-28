namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class STradeBrokerHighestItemLevelPacket : Packet
    {
        const string Name = "S_TRADE_BROKER_HIGHEST_ITEM_LEVEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new STradeBrokerHighestItemLevelPacket();
        }

        [PacketField]
        public uint ItemLevel { get; set; }
    }
}
