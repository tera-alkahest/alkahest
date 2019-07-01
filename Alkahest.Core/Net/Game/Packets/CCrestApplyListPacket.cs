using Alkahest.Core.Collections;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CREST_APPLY_LIST")]
    public sealed class CCrestApplyListPacket : SerializablePacket
    {
        public sealed class CrestInfo
        {
            public uint CrestId { get; set; }
        }

        public NoNullList<CrestInfo> Crests { get; } = new NoNullList<CrestInfo>();

        public byte Unknown1 { get; set; }
    }
}
