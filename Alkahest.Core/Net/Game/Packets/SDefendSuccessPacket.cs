using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_DEFEND_SUCCESS")]
    public sealed class SDefendSuccessPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public SkillId BlockedSkill { get; set; }

        public int Unknown1 { get; set; }

        public bool IsPerfectBlock { get; set; }
    }
}
