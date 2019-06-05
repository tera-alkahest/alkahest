using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_SHOW_INVEN")]
    public sealed class CShowInventoryPacket : SerializablePacket
    {
        public int Unknown1 { get; set; }
    }
}
