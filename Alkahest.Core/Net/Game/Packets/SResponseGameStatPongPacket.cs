using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
