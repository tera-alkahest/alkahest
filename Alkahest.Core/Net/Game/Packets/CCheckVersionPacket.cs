using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CHECK_VERSION")]
    public sealed class CCheckVersionPacket : SerializablePacket
    {
        public sealed class VersionInfo
        {
            public uint Index { get; set; }

            public uint Value { get; set; }
        }

        public List<VersionInfo> Versions { get; } = new List<VersionInfo>();
    }
}
