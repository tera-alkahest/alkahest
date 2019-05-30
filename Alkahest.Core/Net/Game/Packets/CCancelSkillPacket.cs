using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CCancelSkillPacket : Packet
    {
        const string Name = "C_CANCEL_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCancelSkillPacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public uint Type { get; set; }
    }
}
