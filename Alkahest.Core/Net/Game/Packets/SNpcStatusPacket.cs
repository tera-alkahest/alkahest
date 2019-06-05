using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_NPC_STATUS")]
    public sealed class SNpcStatusPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public bool IsEnraged { get; set; }

        public uint EnrageTimeRemaining { get; set; }

        public HPLevel HPLevel { get; set; }

        public short Unknown1 { get; set; }

        public GameId Target { get; set; }

        public NpcStatus Status { get; set; }

        public short Unknown2 { get; set; }
    }
}
