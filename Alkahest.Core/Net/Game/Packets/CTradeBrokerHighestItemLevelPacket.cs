namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CTradeBrokerHighestItemLevelPacket : Packet
    {
        const string Name = "C_TRADE_BROKER_HIGHEST_ITEM_LEVEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CTradeBrokerHighestItemLevelPacket();
        }
    }
}
