using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LOAD_CLIENT_USER_SETTING")]
    public sealed class SLoadClientUserSettingPacket : SerializablePacket
    {
        public List<byte> Data { get; set; } = new List<byte>();
    }
}
