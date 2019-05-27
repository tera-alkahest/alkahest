using Alkahest.Core.Game;
using System.Numerics;

namespace Alkahest.Core.Net.Protocol.Packets
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
        public uint Timestamp { get; set; }

        [PacketField]
        public Vector3 Controls { get; set; }

        [PacketField]
        public Vector3 Direction { get; set; }
    }
}
