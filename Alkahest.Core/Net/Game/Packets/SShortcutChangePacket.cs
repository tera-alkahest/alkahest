using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SHORTCUT_CHANGE")]
    public sealed class SShortcutChangePacket : SerializablePacket
    {
        public uint HuntingZoneId { get; set; }

        public uint ShortcutSetId { get; set; }

        public bool IsEnabled { get; set; }
    }
}
