using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SDefendSuccessPacket : Packet
    {
        const string Name = "S_DEFEND_SUCCESS";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SDefendSuccessPacket();
        }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public SkillId BlockedSkill { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }

        [PacketField]
        public bool IsPerfectBlock { get; set; }
    }
}
