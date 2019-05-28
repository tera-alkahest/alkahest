namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SGuardPKPolicyPacket : Packet
    {
        const string Name = "S_GUARD_PK_POLICY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SGuardPKPolicyPacket();
        }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
