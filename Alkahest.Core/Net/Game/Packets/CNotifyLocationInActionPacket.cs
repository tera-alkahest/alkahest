using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_NOTIFY_LOCATION_IN_ACTION")]
    public sealed class CNotifyLocationInActionPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }

        public uint StageListIndex { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }
    }
}
