using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CServerTimePacket : Packet
    {
        const string Name = "C_SERVER_TIME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CServerTimePacket();
        }
    }
}
