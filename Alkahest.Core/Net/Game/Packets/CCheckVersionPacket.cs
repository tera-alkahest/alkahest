using Alkahest.Core.Collections;
using Alkahest.Core.Net.Game.Serialization;

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

        public NoNullList<VersionInfo> Versions { get; } = new NoNullList<VersionInfo>();
    }
}
