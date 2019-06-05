using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_CHECK_USERNAME")]
    public sealed class CCheckUserNamePacket : SerializablePacket
    {
        public string UserName { get; set; }
    }
}
