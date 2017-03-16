namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SLoadClientUserSettingPacket : Packet
    {
        const string Name = "S_LOAD_CLIENT_USER_SETTING";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoadClientUserSettingPacket();
        }

        [PacketField]
        internal ushort DataOffset { get; set; }

        [PacketField]
        internal ushort DataCount { get; set; }

        [PacketField]
        public byte[] Data { get; set; }
    }
}
