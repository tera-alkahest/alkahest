using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
        public ulong CurrentHP { get; set; }

        [PacketField]
        public ulong MaxHP { get; set; }

        [PacketField]
        public long HPDifference { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public uint AbnormalityId { get; set; }
    }
}
