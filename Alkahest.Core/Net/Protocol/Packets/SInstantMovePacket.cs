using System.Numerics;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SInstantMovePacket : Packet
    {
        const string Name = "S_INSTANT_MOVE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SInstantMovePacket();
        }

        [PacketField]
        public EntityId Source { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public Angle Direction { get; set; }
    }
}
