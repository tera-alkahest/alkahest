using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

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

        public List<PackageInfo> Packages { get; } = new List<PackageInfo>();
    }
}
