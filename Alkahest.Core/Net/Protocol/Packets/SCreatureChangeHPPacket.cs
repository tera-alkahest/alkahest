using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SCreatureChangeHPPacket : Packet
    {
        const string Name = "S_CREATURE_CHANGE_HP";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCreatureChangeHPPacket();
        }

        [PacketField]
        public uint CurrentHP { get; set; }

        [PacketField]
        public uint MaxHP { get; set; }

        [PacketField]
        public int HPDifference { get; set; }

        [PacketField]
        public uint Type { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }
    }
}
