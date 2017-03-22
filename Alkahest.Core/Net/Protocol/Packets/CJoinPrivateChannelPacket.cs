namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CJoinPrivateChannelPacket : Packet
    {
        const string Name = "C_JOIN_PRIVATE_CHANNEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CJoinPrivateChannelPacket();
        }

        [PacketField]
        internal ushort ChannelNameOffset { get; set; }

        [PacketField]
        public ushort Password { get; set; }

        [PacketField]
        public string ChannelName { get; set; }
    }
}
