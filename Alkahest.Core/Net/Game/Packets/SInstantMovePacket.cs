using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_INSTANT_MOVE")]
    public sealed class SInstantMovePacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public Vector3 Destination { get; set; }

        public Angle Direction { get; set; }
    }
}
