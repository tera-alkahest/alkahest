using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_PLAYER_LOCATION")]
    public sealed class CPlayerLocationPacket : SerializablePacket
    {
        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public Angle LookDirection { get; set; }

        public Vector3 Destination { get; set; }

        public MovementKind Kind { get; set; }

        public ushort JumpDistance { get; set; }

        public bool IsInShuttle { get; set; }

        public uint OperatingSystemUpTime { get; set; }
    }
}
