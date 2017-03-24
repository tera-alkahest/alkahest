using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SUnmountVehiclePacket : Packet
    {
        const string Name = "S_UNMOUNT_VEHICLE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SUnmountVehiclePacket();
        }

        [PacketField]
        public EntityId Source { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }
    }
}
