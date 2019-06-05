using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CREST_APPLY_LIST")]
    public sealed class CCrestApplyListPacket : SerializablePacket
    {
        public sealed class CrestInfo
        {
            public uint CrestId { get; set; }
        }

        public List<CrestInfo> Crests { get; } = new List<CrestInfo>();

        public byte Unknown1 { get; set; }
    }
}
