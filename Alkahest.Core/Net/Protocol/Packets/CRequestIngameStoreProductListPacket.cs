namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CRequestIngameStoreProductListPacket : Packet
    {
        const string Name = "C_REQUEST_INGAMESTORE_PRODUCT_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CRequestIngameStoreProductListPacket();
        }
    }
}

