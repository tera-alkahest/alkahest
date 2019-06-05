using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SUpdateEventSeedStatePacket : Packet
    {
        const string Name = "S_UPDATE_EVENT_SEED_STATE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SUpdateEventSeedStatePacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }
    }
}
