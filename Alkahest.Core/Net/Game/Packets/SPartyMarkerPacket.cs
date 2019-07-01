using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_MARKER")]
    public sealed class SPartyMarkerPacket : SerializablePacket
    {
        public sealed class PartyMarkerInfo
        {
            public PartyMarkerColor Color { get; set; }

            public GameId Target { get; set; }
        }

        public NoNullList<PartyMarkerInfo> PartyMarkers { get; } =
            new NoNullList<PartyMarkerInfo>();
    }
}
