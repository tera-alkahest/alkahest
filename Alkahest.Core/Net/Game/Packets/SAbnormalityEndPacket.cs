using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ABNORMALITY_END")]
    public sealed class SAbnormalityEndPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public uint AbnormalityId { get; set; }
    }
}
