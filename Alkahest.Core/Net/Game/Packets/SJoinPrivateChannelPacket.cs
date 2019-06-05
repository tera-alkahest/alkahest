using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_JOIN_PRIVATE_CHANNEL")]
    public sealed class SJoinPrivateChannelPacket : SerializablePacket
    {
        [PacketFieldOptions(IsUnknownArray = true)]
        public ushort Unknown1Count { get; set; }

        [PacketFieldOptions(IsUnknownArray = true)]
        public ushort Unknown1Offset { get; set; }

        public string ChannelName { get; set; }

        public uint Index { get; set; }

        public uint ChannelId { get; set; }
    }
}
