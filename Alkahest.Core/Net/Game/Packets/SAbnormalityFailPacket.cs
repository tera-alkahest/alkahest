using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ABNORMALITY_FAIL")]
    public sealed class SAbnormalityFailPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public uint AbnormalityId { get; set; }

        public byte Unknown1 { get; set; }

        public byte Unknown2 { get; set; }

        public byte Unknown3 { get; set; }
    }
}
