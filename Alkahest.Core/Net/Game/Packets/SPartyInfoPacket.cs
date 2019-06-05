using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPartyInfoPacket : Packet
    {
        const string Name = "S_PARTY_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPartyInfoPacket();
        }

        [PacketField]
        public GameId Leader { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public short Unknown2 { get; set; }

        [PacketField]
        public short Unknown3 { get; set; }

        [PacketField]
        public byte Unknown4 { get; set; }
    }
}
