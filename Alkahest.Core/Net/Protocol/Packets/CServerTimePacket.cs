namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CServerTimePacket : Packet
    {
        const string Name = "C_SERVER_TIME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CServerTimePacket();
        }
    }
}
