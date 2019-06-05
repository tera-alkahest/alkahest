using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
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
        public Angle LookDirection { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public MovementKind Kind { get; set; }

        [PacketField]
        public ushort JumpDistance { get; set; }

        [PacketField]
        public bool IsInShuttle { get; set; }

        [PacketField]
        public uint OperatingSystemUpTime { get; set; }
    }
}
