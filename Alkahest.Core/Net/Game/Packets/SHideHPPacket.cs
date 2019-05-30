using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SHideHPPacket : Packet
    {
        const string Name = "S_HIDE_HP";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SHideHPPacket();
        }

        [PacketField]
        public GameId Target { get; set; }
    }
}
