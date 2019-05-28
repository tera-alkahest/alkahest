using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPrivateChannelNoticePacket : Packet
    {
        const string Name = "S_PRIVATE_CHANNEL_NOTICE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPrivateChannelNoticePacket();
        }

        [PacketField]
        public string UserName { get; set; }

        [PacketField]
        public uint ChannelId { get; set; }

        [PacketField]
        public uint Event { get; set; }

        public string Message { get; set; }

        internal override void OnDeserialize(PacketSerializer serializer)
        {
            Message = serializer.SystemMessages.CodeToName[(ushort)Event];
        }

        internal override void OnSerialize(PacketSerializer serializer)
        {
            Event = serializer.SystemMessages.NameToCode[Message];
        }
    }
}
