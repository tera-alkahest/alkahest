using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_PLAYER_FLYING_LOCATION")]
    public sealed class CPlayerFlyingLocationPacket : SerializablePacket
    {
        public FlyingMovementKind Kind { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Destination { get; set; }

        public uint OperatingSystemUpTime { get; set; }

        public Vector3 ControlDirection { get; set; }

        public Vector3 DestinationDirection { get; set; }
    }
}
