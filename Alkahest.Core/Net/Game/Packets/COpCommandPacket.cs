using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_OP_COMMAND")]
    public sealed class COpCommandPacket : SerializablePacket
    {
        public string Command { get; set; }
    }
}
