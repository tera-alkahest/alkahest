using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class CCanLockOnTargetPacket : Packet
    {
        const string Name = "C_CAN_LOCKON_TARGET";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCanLockOnTargetPacket();
        }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }
    }
}
