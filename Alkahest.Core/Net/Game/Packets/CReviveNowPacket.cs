using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_REVIVE_NOW")]
    public sealed class CReviveNowPacket : SerializablePacket
    {
        public RevivalKind Type { get; set; }

        public int Unknown1 { get; set; }
    }
}
