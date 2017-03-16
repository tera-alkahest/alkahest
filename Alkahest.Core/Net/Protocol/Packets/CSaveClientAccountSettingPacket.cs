namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CSaveClientAccountSettingPacket : Packet
    {
        const string Name = "C_SAVE_CLIENT_ACCOUNT_SETTING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CSaveClientAccountSettingPacket();
        }

        [PacketField]
        internal ushort DataOffset { get; set; }

        [PacketField]
        internal ushort DataCount { get; set; }

        [PacketField]
        public byte[] Data { get; set; }
    }
}
