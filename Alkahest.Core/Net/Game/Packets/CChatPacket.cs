using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CHAT")]
    public sealed class CChatPacket : SerializablePacket
    {
        public string Message { get; set; }

        public ChatChannel Channel { get; set; }
    }
}
