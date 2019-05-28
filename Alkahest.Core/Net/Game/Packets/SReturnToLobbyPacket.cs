namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SReturnToLobbyPacket : Packet
    {
        const string Name = "S_RETURN_TO_LOBBY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SReturnToLobbyPacket();
        }
    }
}
