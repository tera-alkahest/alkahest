using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SStartClientCustomSkillPacket : Packet
    {
        const string Name = "S_START_CLIENT_CUSTOM_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SStartClientCustomSkillPacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }
    }
}
