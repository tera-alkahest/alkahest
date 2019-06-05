using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CEndMoviePacket : Packet
    {
        const string Name = "C_END_MOVIE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CEndMoviePacket();
        }

        [PacketField]
        public uint MovieGroupId { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
