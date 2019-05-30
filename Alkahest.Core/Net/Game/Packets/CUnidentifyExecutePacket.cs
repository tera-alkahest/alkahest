using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CUnidentifyExecutePacketPacket : Packet
    {
        const string Name = "C_UNIDENTIFY_EXECUTE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CUnidentifyExecutePacketPacket();
        }
    }
}
