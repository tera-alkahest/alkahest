using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PARTY_INFO")]
    public sealed class SPartyInfoPacket : SerializablePacket
    {
        public GameId Leader { get; set; }

        public int Unknown1 { get; set; }

        public short Unknown2 { get; set; }

        public short Unknown3 { get; set; }

        public byte Unknown4 { get; set; }
    }
}
