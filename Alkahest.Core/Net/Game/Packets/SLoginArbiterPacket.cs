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
        public ushort Unknown1 { get; set; }

        [PacketField]
        public ushort Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public ushort Unknown4 { get; set; }

        [PacketField]
        public uint Unknown5 { get; set; }

        [PacketField]
        public ushort Unknown6 { get; set; }

        [PacketField]
        public ushort Unknown7 { get; set; }

        [PacketField]
        public byte Unknown8 { get; set; }
    }
}
