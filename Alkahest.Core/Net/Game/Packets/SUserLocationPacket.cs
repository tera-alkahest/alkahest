using Alkahest.Core.Game;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SUserLocationPacket : Packet
    {
        const string Name = "S_USER_LOCATION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SUserLocationPacket();
        }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public ushort Unknown1 { get; set; }

        [PacketField]
        public ushort Speed { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public MovementKind Kind { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
