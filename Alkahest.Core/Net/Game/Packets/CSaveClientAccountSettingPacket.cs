using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CSaveClientAccountSettingPacket : Packet
    {
        const string Name = "C_SAVE_CLIENT_ACCOUNT_SETTING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CSaveClientAccountSettingPacket();
        }

        [PacketField]
        public List<byte> Data { get; set; } = new List<byte>();
    }
}
