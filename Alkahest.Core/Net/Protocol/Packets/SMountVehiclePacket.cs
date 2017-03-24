using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SMountVehiclePacket : Packet
    {
        const string Name = "S_MOUNT_VEHICLE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SMountVehiclePacket();
        }

        [PacketField]
        public EntityId Source { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
