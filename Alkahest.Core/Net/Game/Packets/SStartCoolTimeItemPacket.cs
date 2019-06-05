using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_START_COOLTIME_ITEM")]
    public sealed class SStartCoolTimeItemPacket : SerializablePacket
    {
        public uint ItemId { get; set; }

        public uint Cooldown { get; set; }
    }
}
