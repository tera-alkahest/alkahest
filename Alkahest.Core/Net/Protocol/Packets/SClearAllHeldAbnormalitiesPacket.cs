namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SClearAllHeldAbnormalitiesPacket : Packet
    {
        const string Name = "S_CLEAR_ALL_HOLDED_ABNORMALITY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SClearAllHeldAbnormalitiesPacket();
        }
    }
}
