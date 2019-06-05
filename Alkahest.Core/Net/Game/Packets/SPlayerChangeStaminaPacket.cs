using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPlayerChangeStaminaPacket : Packet
    {
        const string Name = "S_PLAYER_CHANGE_STAMINA";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPlayerChangeStaminaPacket();
        }

        [PacketField]
        public uint CurrentResource { get; set; }

        [PacketField]
        public uint MaxResource { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public int Unknown2 { get; set; }

        [PacketField]
        public int Unknown3 { get; set; }
    }
}
