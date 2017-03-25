using System.Numerics;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CPlayerLocationPacket : Packet
    {
        const string Name = "C_PLAYER_LOCATION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CPlayerLocationPacket();
        }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public ushort Unknown1 { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public MovementKind Kind { get; set; }

        [PacketField]
        public ushort Speed { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public uint Timestamp { get; set; }
    }
}
