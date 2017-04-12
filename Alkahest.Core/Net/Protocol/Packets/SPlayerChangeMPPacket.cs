using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SPlayerChangeMPPacket : Packet
    {
        const string Name = "S_PLAYER_CHANGE_MP";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPlayerChangeMPPacket();
        }

        [PacketField]
        public uint CurrentMP { get; set; }

        [PacketField]
        public uint MaxMP { get; set; }

        [PacketField]
        public int MPDifference { get; set; }

        [PacketField]
        public uint Type { get; set; }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public EntityId Source { get; set; }
    }
}
