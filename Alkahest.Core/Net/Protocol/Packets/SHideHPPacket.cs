using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
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
        public EntityId Target { get; set; }
    }
}
