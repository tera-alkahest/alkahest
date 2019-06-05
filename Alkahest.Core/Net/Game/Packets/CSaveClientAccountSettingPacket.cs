using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_SAVE_CLIENT_ACCOUNT_SETTING")]
    public sealed class CSaveClientAccountSettingPacket : SerializablePacket
    {
        public List<byte> Data { get; set; } = new List<byte>();
    }
}
