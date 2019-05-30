using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLeavePrivateChannelPacket : Packet
    {
        const string Name = "S_LEAVE_PRIVATE_CHANNEL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLeavePrivateChannelPacket();
        }

        [PacketField]
        public uint ChannelId { get; set; }
    }
}
