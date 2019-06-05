using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CNotifyLocationInDashPacket : Packet
    {
        const string Name = "C_NOTIFY_LOCATION_IN_DASH";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CNotifyLocationInDashPacket();
        }

        [PacketField]
        public SkillId Skill { get; set; }

        [PacketField]
        public uint StageListIndex { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }
    }
}
