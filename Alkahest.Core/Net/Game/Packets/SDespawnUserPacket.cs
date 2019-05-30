using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
        public GameId Target { get; set; }

        [PacketField]
        public DespawnKind Kind { get; set; }
    }
}
