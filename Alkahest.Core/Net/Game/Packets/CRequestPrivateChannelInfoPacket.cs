namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CRequestPrivateChannelInfoPacket : Packet
    {
        const string Name = "C_REQUEST_PRIVATE_CHANNEL_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CRequestPrivateChannelInfoPacket();
        }

        [PacketField]
        public uint ChannelId { get; set; }
    }
}
