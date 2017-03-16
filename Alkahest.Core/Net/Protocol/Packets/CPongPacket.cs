namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CPongPacket : Packet
    {
        const string Name = "C_PONG";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CPongPacket();
        }
    }
}
