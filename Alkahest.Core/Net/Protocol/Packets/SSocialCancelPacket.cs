using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SSocialCancelPacket : Packet
    {
        const string Name = "S_SOCIAL_CANCEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSocialCancelPacket();
        }

        [PacketField]
        public EntityId Source { get; set; }
    }
}
