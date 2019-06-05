using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SServerTimePacket : Packet
    {
        const string Name = "S_SERVER_TIME";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SServerTimePacket();
        }

        [PacketField]
        public ulong ServerTime { get; set; }
    }
}
