using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PRIVATE_CHAT")]
    public sealed class SPrivateChatPacket : SerializablePacket
    {
        public string SenderName { get; set; }

        public string Message { get; set; }

        public uint ChannelId { get; set; }

        public GameId Source { get; set; }
    }
}
