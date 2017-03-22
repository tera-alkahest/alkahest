using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
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
        internal ushort UserNameOffset { get; set; }

        [PacketField]
        public uint ChannelId { get; set; }

        [PacketField]
        public uint Event { get; set; }

        [PacketField]
        public string UserName { get; set; }

        public string Message { get; set; }

        internal override void OnDeserialize(PacketSerializer serializer)
        {
            Message = serializer.SystemMessages.OpCodeToName[(ushort)Event];
        }

        internal override void OnSerialize(PacketSerializer serializer)
        {
            Event = serializer.SystemMessages.NameToOpCode[Message];
        }
    }
}
