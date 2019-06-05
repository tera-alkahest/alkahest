using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SUserFlyingLocationPacket : Packet
    {
        const string Name = "S_USER_FLYING_LOCATION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SUserFlyingLocationPacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public FlyingMovementKind Kind { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public Vector3 ControlDirection { get; set; }

        [PacketField]
        public Vector3 DestinationDirection { get; set; }

        [PacketField]
        public float Speed { get; set; }

        [PacketField]
        public float RotationSpeed { get; set; }
    }
}
