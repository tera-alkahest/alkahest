using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SCrestInfoPacket : Packet
    {
        const string Name = "S_CREST_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCrestInfoPacket();
        }

        public sealed class CrestInfo
        {
            [PacketField]
            public uint CrestId { get; set; }

            [PacketField]
            public bool IsActive { get; set; }
        }

        [PacketField]
        public List<CrestInfo> Crests { get; } = new List<CrestInfo>();

        [PacketField]
        public uint MaxCrestPoints { get; set; }

        [PacketField]
        public uint UsedCrestPoints { get; set; }
    }
}
