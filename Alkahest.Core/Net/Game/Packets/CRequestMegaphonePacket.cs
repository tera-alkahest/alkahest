using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CRequestMegaphonePacket : Packet
    {
        const string Name = "C_REQUEST_MEGAPHONE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CRequestMegaphonePacket();
        }

        [PacketField]
        public string Message { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public int Unknown2 { get; set; }
    }
}
