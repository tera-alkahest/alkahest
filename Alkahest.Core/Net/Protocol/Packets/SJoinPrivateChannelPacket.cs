namespace Alkahest.Core.Net.Protocol.Packets
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

        [PacketField]
        internal ushort Unknown1Count { get; set; }

        [PacketField]
        internal ushort Unknown1Offset { get; set; }

        [PacketField]
        internal ushort ChannelNameOffset { get; set; }

        [PacketField]
        public uint Index { get; set; }

        [PacketField]
        public uint ChannelId { get; set; }

        [PacketField]
        public string ChannelName { get; set; }
    }
}
