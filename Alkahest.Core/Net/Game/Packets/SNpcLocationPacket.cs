using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SNpcLocationPacket : Packet
    {
        const string Name = "S_NPC_LOCATION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SNpcLocationPacket();
        }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public ushort Speed { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public MovementKind Kind { get; set; }
    }
}
