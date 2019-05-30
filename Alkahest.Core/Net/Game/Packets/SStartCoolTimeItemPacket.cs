using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SStartCoolTimeItemPacket : Packet
    {
        const string Name = "S_START_COOLTIME_ITEM";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SStartCoolTimeItemPacket();
        }

        [PacketField]
        public uint ItemId { get; set; }

        [PacketField]
        public uint Cooldown { get; set; }
    }
}
