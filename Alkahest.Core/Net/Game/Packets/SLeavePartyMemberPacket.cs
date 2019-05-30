using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLeavePartyMemberPacket : Packet
    {
        const string Name = "S_LEAVE_PARTY_MEMBER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLeavePartyMemberPacket();
        }

        [PacketField]
        public string UserName { get; set; }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }
    }
}
