using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLoadTopoPacket : Packet
    {
        const string Name = "S_LOAD_TOPO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoadTopoPacket();
        }

        [PacketField]
        public uint Zone { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
