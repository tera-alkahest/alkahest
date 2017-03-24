using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SDecreaseCoolTimeSkillPacket : Packet
    {
        const string Name = "S_DECREASE_COOLTIME_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SDecreaseCoolTimeSkillPacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public uint Cooldown { get; set; }
    }
}
