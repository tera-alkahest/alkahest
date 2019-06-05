using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SOCIAL_CANCEL")]
    public sealed class SSocialCancelPacket : SerializablePacket
    {
        public GameId Source { get; set; }
    }
}
