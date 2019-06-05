using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_INSTANT_DASH")]
    public sealed class SInstantDashPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public GameId Target { get; set; }

        public int Unknown1 { get; set; }

        public Vector3 Destination { get; set; }

        public Angle Direction { get; set; }
    }
}
