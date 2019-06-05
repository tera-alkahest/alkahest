using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ABNORMALITY_FAIL")]
    public sealed class SAbnormalityFailPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public uint AbnormalityId { get; set; }

        public AbnormalityFailureReason Reason { get; set; }

        public bool IsBuff { get; set; }
    }
}
