namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SLoadingScreenControlInfoPacket : Packet
    {
        const string Name = "S_LOADING_SCREEN_CONTROL_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoadingScreenControlInfoPacket();
        }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
