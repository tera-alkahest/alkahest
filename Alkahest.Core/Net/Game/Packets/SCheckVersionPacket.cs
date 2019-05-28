namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SCheckVersionPacket : Packet
    {
        const string Name = "S_CHECK_VERSION";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCheckVersionPacket();
        }

        [PacketField]
        public bool IsCompatible { get; set; }
    }
}
