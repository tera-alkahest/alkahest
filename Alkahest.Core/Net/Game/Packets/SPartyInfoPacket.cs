using Alkahest.Core.Game;

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
        public uint Unknown1 { get; set; }

        [PacketField]
        public ushort Unknown2 { get; set; }

        [PacketField]
        public ushort Unknown3 { get; set; }

        [PacketField]
        public byte Unknown4 { get; set; }
    }
}
