namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPingPacket : Packet
    {
        const string Name = "S_PING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPingPacket();
        }
    }
}
