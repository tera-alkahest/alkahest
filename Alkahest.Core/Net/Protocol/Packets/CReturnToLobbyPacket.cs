namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CReturnToLobbyPacket : Packet
    {
        const string Name = "C_RETURN_TO_LOBBY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CReturnToLobbyPacket();
        }
    }
}
