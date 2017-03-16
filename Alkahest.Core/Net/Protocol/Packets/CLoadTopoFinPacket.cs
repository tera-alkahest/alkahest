namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CLoadTopoFinPacket : Packet
    {
        const string Name = "C_LOAD_TOPO_FIN";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CLoadTopoFinPacket();
        }
    }
}
