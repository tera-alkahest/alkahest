using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SSuccessCounterPacket : Packet
    {
        const string Name = "S_SUCCESS_COUNTER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSuccessCounterPacket();
        }

        [PacketField]
        public GameId Source { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public SkillId DodgeSkill { get; set; }
    }
}
