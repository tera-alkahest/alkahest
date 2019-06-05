using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
        public uint HuntingZoneId { get; set; }

        [PacketField]
        public uint ShortcutSetId { get; set; }

        [PacketField]
        public bool IsEnabled { get; set; }
    }
}
