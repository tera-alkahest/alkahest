using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_UPDATE_EVENT_SEED_STATE")]
    public sealed class SUpdateEventSeedStatePacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public int Unknown1 { get; set; }
    }
}
