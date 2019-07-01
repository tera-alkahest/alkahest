using Alkahest.Core.Collections;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CREST_INFO")]
    public sealed class SCrestInfoPacket : SerializablePacket
    {
        public sealed class CrestInfo
        {
            public uint CrestId { get; set; }

            public bool IsActive { get; set; }
        }

        public NoNullList<CrestInfo> Crests { get; } = new NoNullList<CrestInfo>();

        public uint MaxCrestPoints { get; set; }

        public uint UsedCrestPoints { get; set; }
    }
}
