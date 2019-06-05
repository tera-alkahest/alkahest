using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CREATURE_LIFE")]
    public sealed class SCreatureLifePacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public Vector3 Position { get; set; }

        public bool IsAlive { get; set; }

        public bool IsInShuttle { get; set; }

        public byte Unknown1 { get; set; }

        public byte Unknown2 { get; set; }
    }
}
