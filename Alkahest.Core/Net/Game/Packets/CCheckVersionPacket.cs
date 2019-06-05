using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CCheckVersionPacket : Packet
    {
        const string Name = "C_CHECK_VERSION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCheckVersionPacket();
        }

        public sealed class VersionInfo
        {
            [PacketField]
            public uint Index { get; set; }

            [PacketField]
            public uint Value { get; set; }
        }

        [PacketField]
        public List<VersionInfo> Versions { get; } = new List<VersionInfo>();
    }
}
