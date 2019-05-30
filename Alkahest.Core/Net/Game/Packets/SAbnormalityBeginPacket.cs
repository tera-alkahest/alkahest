using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SAbnormalityBeginPacket : Packet
    {
        const string Name = "S_ABNORMALITY_BEGIN";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAbnormalityBeginPacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public uint AbnormalityId { get; set; }

        [PacketField]
        public uint Duration { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Stacks { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }
    }
}
