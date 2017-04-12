using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCannotStartSkillPacket : Packet
    {
        const string Name = "S_CANNOT_START_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCannotStartSkillPacket();
        }

        [PacketField(IsLocalSkill = true)]
        public SkillId Skill { get; set; }
    }
}
