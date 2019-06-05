using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_NPC_LOCATION")]
    public sealed class SNpcLocationPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public ushort Speed { get; set; }

        public Vector3 Destination { get; set; }

        public MovementKind Kind { get; set; }
    }
}
