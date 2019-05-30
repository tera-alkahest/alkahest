using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SSocialPacket : Packet
    {
        const string Name = "S_SOCIAL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSocialPacket();
        }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public uint Emote { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
