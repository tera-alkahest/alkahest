using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
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
        public GameId Source { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }
    }
}
