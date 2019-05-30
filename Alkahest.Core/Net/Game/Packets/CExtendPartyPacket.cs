using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CExtendPartyPacket : Packet
    {
        const string Name = "C_EXTEND_PARTY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CExtendPartyPacket();
        }

        [PacketField]
        public bool IsRaid { get; set; }
    }
}
