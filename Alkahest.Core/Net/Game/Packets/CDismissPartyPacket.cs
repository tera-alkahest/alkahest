using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CDismissPartyPacket : Packet
    {
        const string Name = "C_DISMISS_PARTY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CDismissPartyPacket();
        }
    }
}
