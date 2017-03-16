namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CGetUserListPacket : Packet
    {
        const string Name = "C_GET_USER_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CGetUserListPacket();
        }
    }
}
