using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_UNMOUNT_VEHICLE")]
    public sealed class SUnmountVehiclePacket : SerializablePacket
    {
        public GameId Source { get; set; }

        [PacketFieldOptions(IsSimpleSkill = true)]
        public SkillId Skill { get; set; }
    }
}
