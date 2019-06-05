using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_MOUNT_VEHICLE")]
    public sealed class SMountVehiclePacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public uint VehicleId { get; set; }

        [PacketFieldOptions(IsSimpleSkill = true)]
        public SkillId Skill { get; set; }

        public byte Unknown1 { get; set; }
    }
}
