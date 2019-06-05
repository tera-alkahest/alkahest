using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ADMIN_HOLD_CHARACTER")]
    public sealed class SAdminHoldCharacterPacket : SerializablePacket
    {
        public byte Unknown1 { get; set; }
    }
}
