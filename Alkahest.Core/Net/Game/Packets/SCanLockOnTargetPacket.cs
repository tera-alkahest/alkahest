using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CAN_LOCKON_TARGET")]
    public sealed class SCanLockOnTargetPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public int Unknown1 { get; set; }

        public SkillId Skill { get; set; }

        public bool CanLockOn { get; set; }
    }
}
