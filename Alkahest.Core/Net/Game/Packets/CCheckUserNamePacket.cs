using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CCheckUserNamePacket : Packet
    {
        const string Name = "C_CHECK_USERNAME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCheckUserNamePacket();
        }

        [PacketField]
        public string UserName { get; set; }
    }
}
