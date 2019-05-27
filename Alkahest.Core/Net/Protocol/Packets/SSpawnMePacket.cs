using Alkahest.Core.Game;
using System.Numerics;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SSpawnMePacket : Packet
    {
        const string Name = "S_SPAWN_ME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSpawnMePacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public bool IsAlive { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
