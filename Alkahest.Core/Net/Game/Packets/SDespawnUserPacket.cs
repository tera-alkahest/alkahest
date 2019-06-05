using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_DESPAWN_USER")]
    public sealed class SDespawnUserPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public DespawnKind Kind { get; set; }
    }
}
