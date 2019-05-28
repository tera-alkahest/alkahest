using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SAbnormalityRefreshPacket : Packet
    {
        const string Name = "S_ABNORMALITY_REFRESH";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAbnormalityRefreshPacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public uint AbnormalityId { get; set; }

        [PacketField]
        public uint Duration { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Stacks { get; set; }
    }
}
