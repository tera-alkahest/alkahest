namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SJoinPrivateChannelPacket : Packet
    {
        const string Name = "S_JOIN_PRIVATE_CHANNEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SJoinPrivateChannelPacket();
        }

        [PacketField(IsUnknownArray = true)]
        public ushort Unknown1Count { get; set; }

        [PacketField(IsUnknownArray = true)]
        public ushort Unknown1Offset { get; set; }

        [PacketField]
        public string ChannelName { get; set; }

        [PacketField]
        public uint Index { get; set; }

        [PacketField]
        public uint ChannelId { get; set; }
    }
}
