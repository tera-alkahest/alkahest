using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLoginAccountInfoPacket : Packet
    {
        const string Name = "S_LOGIN_ACCOUNT_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoginAccountInfoPacket();
        }

        [PacketField]
        public string ServerName { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }
    }
}
