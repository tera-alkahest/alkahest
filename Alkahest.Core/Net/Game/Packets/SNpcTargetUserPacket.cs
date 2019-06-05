using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_NPC_TARGET_USER")]
    public sealed class SNpcTargetUserPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public byte Unknown1 { get; set; }
    }
}
