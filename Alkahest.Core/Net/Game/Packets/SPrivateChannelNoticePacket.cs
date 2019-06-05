using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PRIVATE_CHANNEL_NOTICE")]
    public sealed class SPrivateChannelNoticePacket : SerializablePacket
    {
        public string UserName { get; set; }

        public uint ChannelId { get; set; }

        public uint Event { get; set; }

        [PacketFieldOptions(Skip = true)]
        public string MessageName { get; set; }

        internal override void OnDeserialize(PacketSerializer serializer)
        {
            MessageName = serializer.SystemMessages.CodeToName[(ushort)Event];
        }

        internal override void OnSerialize(PacketSerializer serializer)
        {
            Event = serializer.SystemMessages.NameToCode[MessageName];
        }
    }
}
