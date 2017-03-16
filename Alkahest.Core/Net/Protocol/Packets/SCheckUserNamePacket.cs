namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SCheckUserNamePacket : Packet
    {
        const string Name = "S_CHECK_USERNAME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCheckUserNamePacket();
        }

        [PacketField]
        public bool IsAvailable { get; set; }
    }
}
