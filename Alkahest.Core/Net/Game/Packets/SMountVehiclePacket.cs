using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
        public GameId Source { get; set; }

        [PacketField]
        public uint VehicleId { get; set; }

        [PacketField(IsSimpleSkill = true)]
        public SkillId Skill { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }
    }
}
