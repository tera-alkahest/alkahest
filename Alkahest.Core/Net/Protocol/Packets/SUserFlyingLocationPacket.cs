using System.Numerics;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
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
        public Vector3 Controls { get; set; }

        [PacketField]
        public Vector3 Direction { get; set; }

        [PacketField]
        public SkillId Unknown1 { get; set; }

        [PacketField]
        public SkillId Unknown2 { get; set; }
    }
}
