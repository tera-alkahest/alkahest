using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SUserStatusPacket : Packet
    {
        const string Name = "S_USER_STATUS";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SUserStatusPacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public UserStatus Status { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
