namespace Alkahest.Core.Net.Protocol.Packets
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
