using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
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
