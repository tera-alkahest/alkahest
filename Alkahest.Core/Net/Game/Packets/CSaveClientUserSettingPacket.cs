using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_SAVE_CLIENT_USER_SETTING")]
    public sealed class CSaveClientUserSettingPacket : SerializablePacket
    {
        public List<byte> Data { get; set; } = new List<byte>();
    }
}
