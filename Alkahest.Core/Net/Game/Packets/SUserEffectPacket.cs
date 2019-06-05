using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_USER_EFFECT")]
    public sealed class SUserEffectPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public GameId Source { get; set; }

        public UserEffectKind Effect { get; set; }

        public UserEffectOperation Operation { get; set; }
    }
}
