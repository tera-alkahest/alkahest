using Alkahest.Core.Game;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CPartyMarkerPacket : Packet
    {
        const string Name = "C_PARTY_MARKER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CPartyMarkerPacket();
        }

        public sealed class PartyMarkerInfo
        {
            [PacketField]
            public PartyMarkerColor Color { get; set; }

            [PacketField]
            public GameId Target { get; set; }
        }

        [PacketField]
        public List<PartyMarkerInfo> PartyMarkers { get; } = new List<PartyMarkerInfo>();
    }
}
