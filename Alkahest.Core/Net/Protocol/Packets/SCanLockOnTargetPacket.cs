using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SCanLockOnTargetPacket : Packet
    {
        const string Name = "S_CAN_LOCKON_TARGET";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SCanLockOnTargetPacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public bool CanLockOn { get; set; }
    }
}
