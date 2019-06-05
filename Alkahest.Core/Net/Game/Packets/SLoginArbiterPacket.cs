using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLoginArbiterPacket : Packet
    {
        const string Name = "S_LOGIN_ARBITER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoginArbiterPacket();
        }

        [PacketField]
        public bool IsSuccessful { get; set; }

        [PacketField]
        public bool IsInQueue { get; set; }

        [PacketField]
        public uint Status { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public ClientRegion Region { get; set; }

        [PacketField]
        public bool DisablePlayerVersusPlayer { get; set; }

        [PacketField]
        public short Unknown2 { get; set; }

        [PacketField]
        public short Unknown3 { get; set; }
    }
}
