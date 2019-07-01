using Alkahest.Core.Collections;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ACCOUNT_PACKAGE_LIST")]
    public sealed class SAccountPackageListPacket : SerializablePacket
    {
        public sealed class PackageInfo
        {
            public uint PackageId { get; set; }

            public ulong ExpirationTime { get; set; }
        }

        public NoNullList<PackageInfo> Packages { get; } = new NoNullList<PackageInfo>();
    }
}
