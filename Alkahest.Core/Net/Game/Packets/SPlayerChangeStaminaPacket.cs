using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PLAYER_CHANGE_STAMINA")]
    public sealed class SPlayerChangeStaminaPacket : SerializablePacket
    {
        public uint CurrentResource { get; set; }

        public uint MaxResource { get; set; }

        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }
    }
}
