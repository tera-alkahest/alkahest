using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SNpcMenuSelectPacket : Packet
    {
        const string Name = "S_NPC_MENU_SELECT";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SNpcMenuSelectPacket();
        }

        [PacketField]
        public uint Type { get; set; }

    }
}
