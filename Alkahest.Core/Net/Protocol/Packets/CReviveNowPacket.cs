using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CReviveNowPacket : Packet
    {
        const string Name = "C_REVIVE_NOW";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CReviveNowPacket();
        }

        [PacketField]
        public uint Type { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }
    }
}
