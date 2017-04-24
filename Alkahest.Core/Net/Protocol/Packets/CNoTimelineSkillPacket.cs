using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CNoTimelinePacket : Packet
    {
        const string Name = "C_NOTIMELINE_SKILL";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CNoTimelinePacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }
    }
}
