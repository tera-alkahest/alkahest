namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CRequestUserItemLevelInfoPacket : Packet
    {
        const string Name = "C_REQUEST_USER_ITEMLEVEL_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CRequestUserItemLevelInfoPacket();
        }
    }
}

