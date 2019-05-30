using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SInstantDashPacket : Packet
    {
        const string Name = "S_INSTANT_DASH";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SInstantDashPacket();
        }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public Vector3 Destination { get; set; }

        [PacketField]
        public Angle Direction { get; set; }
    }
}
