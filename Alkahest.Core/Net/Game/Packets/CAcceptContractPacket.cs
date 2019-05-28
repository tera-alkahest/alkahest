namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CAcceptContractPacket : Packet
    {
        const string Name = "C_ACCEPT_CONTRACT";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CAcceptContractPacket();
        }

        [PacketField]
        public uint Type { get; set; }

        [PacketField]
        public uint Id { get; set; }
    }
}
