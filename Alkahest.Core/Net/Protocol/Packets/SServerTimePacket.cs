namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SServerTimePacket : Packet
    {
        const string Name = "S_SERVER_TIME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SServerTimePacket();
        }

        [PacketField]
        public ulong Time { get; set; }
    }
}
