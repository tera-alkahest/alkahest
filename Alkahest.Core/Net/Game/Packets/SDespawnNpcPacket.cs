using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_DESPAWN_NPC")]
    public sealed class SDespawnNpcPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public Vector3 Position { get; set; }

        public DespawnKind Kind { get; set; }

        public int Unknown1 { get; set; }
    }
}
