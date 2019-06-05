using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SSwitchInfoPacket : Packet
    {
        const string Name = "S_SWITCH_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSwitchInfoPacket();
        }

        [PacketField]
        public SkillId OnSkill { get; set; }

        [PacketField]
        public SkillId OffSkill { get; set; }

        [PacketField]
        public bool IsEnabled { get; set; }
    }
}
