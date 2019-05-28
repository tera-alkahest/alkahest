using Alkahest.Core.Game;

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
        public uint Unknown1 { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }
    }
}
