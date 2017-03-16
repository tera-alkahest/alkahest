namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SLoadClientAccountSettingPacket : Packet
    {
        const string Name = "S_LOAD_CLIENT_ACCOUNT_SETTING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoadClientAccountSettingPacket();
        }

        [PacketField]
        internal ushort DataOffset { get; set; }

        [PacketField]
        internal ushort DataCount { get; set; }

        [PacketField]
        public byte[] Data { get; set; }
    }
}
