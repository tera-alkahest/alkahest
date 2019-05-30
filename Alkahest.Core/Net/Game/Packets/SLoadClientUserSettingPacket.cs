using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLoadClientUserSettingPacket : Packet
    {
        const string Name = "S_LOAD_CLIENT_USER_SETTING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoadClientUserSettingPacket();
        }

        [PacketField]
        public List<byte> Data { get; set; } = new List<byte>();
    }
}
