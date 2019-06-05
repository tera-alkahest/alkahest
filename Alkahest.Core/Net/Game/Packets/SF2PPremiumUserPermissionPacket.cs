using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SF2PPremiumUserPermissionPacket : Packet
    {
        const string Name = "S_F2P_PremiumUser_Permission";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SF2PPremiumUserPermissionPacket();
        }

        public sealed class PermissionInfo
        {
            [PacketField]
            public short Unknown1 { get; set; }

            [PacketField]
            public short Unknown2 { get; set; }
        }

        [PacketField]
        public List<PermissionInfo> Permissions { get; } = new List<PermissionInfo>();

        [PacketField]
        public int Unknown3 { get; set; }

        [PacketField]
        public short Unknown4 { get; set; }

        [PacketField]
        public short Unknown5 { get; set; }

        [PacketField]
        public short Unknown6 { get; set; }

        [PacketField]
        public short Unknown7 { get; set; }
    }
}
