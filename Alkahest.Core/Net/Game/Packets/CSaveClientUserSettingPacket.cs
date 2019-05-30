using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CSaveClientUserSettingPacket : Packet
    {
        const string Name = "C_SAVE_CLIENT_USER_SETTING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CSaveClientUserSettingPacket();
        }

        [PacketField]
        public List<byte> Data { get; set; } = new List<byte>();
    }
}
