using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SAccountPackageListPacket : Packet
    {
        const string Name = "S_ACCOUNT_PACKAGE_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAccountPackageListPacket();
        }

        public sealed class PackageInfo
        {
            [PacketField]
            public uint PackageId { get; set; }

            [PacketField]
            public int TimeRemaining { get; set; }

            [PacketField]
            public uint Unknown1 { get; set; }
        }

        [PacketField]
        public List<PackageInfo> Packages { get; } = new List<PackageInfo>();
    }
}
