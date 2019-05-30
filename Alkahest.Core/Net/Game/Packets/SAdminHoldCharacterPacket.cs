using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SAdminHoldCharacterPacket : Packet
    {
        const string Name = "S_ADMIN_HOLD_CHARACTER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SAdminHoldCharacterPacket();
        }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
