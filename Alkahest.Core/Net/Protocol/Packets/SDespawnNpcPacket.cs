using System.Numerics;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SDespawnNpcPacket : Packet
    {
        const string Name = "S_DESPAWN_NPC";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SDespawnNpcPacket();
        }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public DespawnKind Kind { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }
    }
}
