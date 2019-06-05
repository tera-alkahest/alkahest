using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPlayMoviePacket : Packet
    {
        const string Name = "S_PLAY_MOVIE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPlayMoviePacket();
        }

        [PacketField]
        public uint MovieGroupId { get; set; }
    }
}
