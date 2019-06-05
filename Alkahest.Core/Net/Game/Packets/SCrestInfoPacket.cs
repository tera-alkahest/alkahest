using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

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

        public List<CrestInfo> Crests { get; } = new List<CrestInfo>();

        public uint MaxCrestPoints { get; set; }

        public uint UsedCrestPoints { get; set; }
    }
}
