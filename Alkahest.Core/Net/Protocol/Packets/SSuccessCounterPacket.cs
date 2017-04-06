using Alkahest.Core.Data;

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
        public EntityId Source { get; set; }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public SkillId DodgeSkill { get; set; }
    }
}
