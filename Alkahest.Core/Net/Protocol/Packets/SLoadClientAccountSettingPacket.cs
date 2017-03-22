using System.Collections.Generic;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SLoadClientAccountSettingPacket : Packet
    {
        const string Name = "S_LOAD_CLIENT_ACCOUNT_SETTING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoadClientAccountSettingPacket();
        }

        [PacketField]
        public List<byte> Data { get; set; } = new List<byte>();
    }
}
