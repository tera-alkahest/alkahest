using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_START_CLIENT_CUSTOM_SKILL")]
    public sealed class SStartClientCustomSkillPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public SkillId Skill { get; set; }
    }
}
