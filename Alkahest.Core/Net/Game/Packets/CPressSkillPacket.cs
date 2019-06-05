using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_PRESS_SKILL")]
    public sealed class CPressSkillPacket : SerializablePacket
    {
        public SkillId Skill { get; set; }

        public bool IsPress { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }
    }
}
