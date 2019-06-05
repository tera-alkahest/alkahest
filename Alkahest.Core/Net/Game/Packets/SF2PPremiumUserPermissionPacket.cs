using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_F2P_PremiumUser_Permission")]
    public sealed class SF2PPremiumUserPermissionPacket : SerializablePacket
    {
        public sealed class PermissionInfo
        {
            public short Unknown1 { get; set; }

            public short Unknown2 { get; set; }
        }

        public List<PermissionInfo> Permissions { get; } = new List<PermissionInfo>();

        public int Unknown3 { get; set; }

        public short Unknown4 { get; set; }

        public short Unknown5 { get; set; }

        public short Unknown6 { get; set; }

        public short Unknown7 { get; set; }
    }
}
