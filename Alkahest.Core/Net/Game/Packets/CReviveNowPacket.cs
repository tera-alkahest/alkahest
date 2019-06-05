using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CReviveNowPacket : Packet
    {
        const string Name = "C_REVIVE_NOW";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CReviveNowPacket();
        }

        [PacketField]
        public RevivalKind Type { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }
    }
}
