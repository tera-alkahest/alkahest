using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SUCCESS_COUNTER")]
    public sealed class SSuccessCounterPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public GameId Target { get; set; }

        [PacketFieldOptions(IsSimpleSkill = true)]
        public SkillId DodgeSkill { get; set; }
    }
}
