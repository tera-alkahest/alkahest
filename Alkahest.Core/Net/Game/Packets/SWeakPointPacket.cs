using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_WEAK_POINT")]
    public sealed class SWeakPointPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public uint RunemarksAdded { get; set; }

        public uint RunemarksRemoved { get; set; }

        public RunemarkEventKind EventKind { get; set; }

        [PacketFieldOptions(IsSimpleSkill = true)]
        public SkillId Skill { get; set; }
    }
}
