using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SDespawnUserPacket : Packet
    {
        const string Name = "S_DESPAWN_USER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SDespawnUserPacket();
        }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public DespawnType Type { get; set; }
    }
}
