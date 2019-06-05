using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SNpcStatusPacket : Packet
    {
        const string Name = "S_NPC_STATUS";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SNpcStatusPacket();
        }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public bool IsEnraged { get; set; }

        [PacketField]
        public uint EnrageTimeRemaining { get; set; }

        [PacketField]
        public HPLevel HPLevel { get; set; }

        [PacketField]
        public short Unknown1 { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public NpcStatus Status { get; set; }

        [PacketField]
        public short Unknown2 { get; set; }
    }
}
