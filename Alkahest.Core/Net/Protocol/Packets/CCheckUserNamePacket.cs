namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCheckUserNamePacket : Packet
    {
        const string Name = "C_CHECK_USERNAME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCheckUserNamePacket();
        }

        [PacketField]
        internal ushort NameOffset { get; set; }

        [PacketField]
        public string UserName { get; set; }
    }
}
