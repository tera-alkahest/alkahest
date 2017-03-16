using System.Numerics;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SActionEndPacket : Packet
    {
        const string Name = "S_ACTION_END";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SActionEndPacket();
        }

        [PacketField]
        public EntityId Source { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public uint Model { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public uint Type { get; set; }

        [PacketField]
        public uint CorrelationId { get; set; }
    }
}
