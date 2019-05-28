namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CRequestGameStatPingPacket : Packet
    {
        const string Name = "C_REQUEST_GAMESTAT_PING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CRequestGameStatPingPacket();
        }
    }
}
