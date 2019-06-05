using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CNoTimelineSkillPacket : Packet
    {
        const string Name = "C_NOTIMELINE_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CNoTimelineSkillPacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }
    }
}
