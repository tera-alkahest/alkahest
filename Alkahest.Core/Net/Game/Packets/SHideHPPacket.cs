using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_HIDE_HP")]
    public sealed class SHideHPPacket : SerializablePacket
    {
        public GameId Target { get; set; }
    }
}
