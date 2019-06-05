using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
        public GameId Source { get; set; }

        [PacketField(IsSimpleSkill = true)]
        public SkillId Skill { get; set; }
    }
}
