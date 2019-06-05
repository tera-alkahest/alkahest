using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CSocialPacket : Packet
    {
        const string Name = "C_SOCIAL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CSocialPacket();
        }

        [PacketField]
        public uint SocialId { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
