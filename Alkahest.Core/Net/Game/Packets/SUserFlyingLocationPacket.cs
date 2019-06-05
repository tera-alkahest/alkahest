using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_USER_FLYING_LOCATION")]
    public sealed class SUserFlyingLocationPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public FlyingMovementKind Kind { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Destination { get; set; }

        public Vector3 ControlDirection { get; set; }

        public Vector3 DestinationDirection { get; set; }

        public float Speed { get; set; }

        public float RotationSpeed { get; set; }
    }
}
