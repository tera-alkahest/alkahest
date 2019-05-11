using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SPrivateChatPacket : Packet
    {
        const string Name = "S_PRIVATE_CHAT";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPrivateChatPacket();
        }

        [PacketField]
        public string SenderName { get; set; }

        [PacketField]
        public string Message { get; set; }

        [PacketField]
        public uint ChannelId { get; set; }

        [PacketField]
        public GameId Source { get; set; }
    }
}
