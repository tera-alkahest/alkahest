using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_NPC_MENU_SELECT")]
    public sealed class SNpcMenuSelectPacket : SerializablePacket
    {
        public int Unknown1 { get; set; }
    }
}
