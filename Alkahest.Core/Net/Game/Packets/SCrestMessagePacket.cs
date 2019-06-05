using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SCrestMessagePacket : Packet
    {
        const string Name = "S_CREST_MESSAGE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCrestMessagePacket();
        }

        [PacketField]
        public uint CrestId { get; set; }

        [PacketField]
        public CrestMessageKind Kind { get; set; }

        [PacketField(IsSimpleSkill = true)]
        public SkillId Skill { get; set; }
    }
}
