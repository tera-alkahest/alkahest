using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_ADMIN")]
    public sealed class CAdminPacket : SerializablePacket
    {
        public string Command { get; set; }
    }
}
