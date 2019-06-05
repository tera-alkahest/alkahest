using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SPAWN_ME")]
    public sealed class SSpawnMePacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public bool IsAlive { get; set; }

        public byte Unknown1 { get; set; }
    }
}
