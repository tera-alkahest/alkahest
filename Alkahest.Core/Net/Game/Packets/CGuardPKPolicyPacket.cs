using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CGuardPKPolicyPacket : Packet
    {
        const string Name = "C_GUARD_PK_POLICY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CGuardPKPolicyPacket();
        }
    }
}
