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

        public sealed class Permission
        {
            [PacketField]
            public ushort Unknown1 { get; set; }

            [PacketField]
            public ushort Unknown2 { get; set; }
        }

        [PacketField]
        public List<Permission> Permissions { get; } = new List<Permission>();

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public ushort Unknown4 { get; set; }

        [PacketField]
        public ushort Unknown5 { get; set; }

        [PacketField]
        public ushort Unknown6 { get; set; }

        [PacketField]
        public ushort Unknown7 { get; set; }
    }
}
