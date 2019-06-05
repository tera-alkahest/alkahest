using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SCreatureLifePacket : Packet
    {
        const string Name = "S_CREATURE_LIFE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCreatureLifePacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public bool IsAlive { get; set; }

        [PacketField]
        public bool IsInShuttle { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
