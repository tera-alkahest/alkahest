namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SShortcutChangePacket : Packet
    {
        const string Name = "S_SHORTCUT_CHANGE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SShortcutChangePacket();
        }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public bool Unknown3 { get; set; }
    }
}
