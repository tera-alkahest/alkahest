using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CPlayerFlyingLocationPacket : Packet
    {
        const string Name = "C_PLAYER_FLYING_LOCATION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CPlayerFlyingLocationPacket();
        }

        [PacketField]
        public FlyingMovementKind Kind { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public uint OperatingSystemUpTime { get; set; }

        [PacketField]
        public Vector3 ControlDirection { get; set; }

        [PacketField]
        public Vector3 DestinationDirection { get; set; }
    }
}
