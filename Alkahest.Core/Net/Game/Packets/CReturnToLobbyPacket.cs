using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
