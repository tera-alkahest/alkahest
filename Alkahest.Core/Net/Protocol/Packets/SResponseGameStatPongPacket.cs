namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SResponseGameStatPongPacket : Packet
    {
        const string Name = "S_RESPONSE_GAMESTAT_PONG";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SResponseGameStatPongPacket();
        }
    }
}
