using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_TRADE_BROKER_HIGHEST_ITEM_LEVEL")]
    public sealed class STradeBrokerHighestItemLevelPacket : SerializablePacket
    {
        public uint ItemLevel { get; set; }
    }
}
