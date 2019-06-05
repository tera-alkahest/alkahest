using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ACTION_END")]
    public sealed class SActionEndPacket : SerializablePacket
    {
        public GameId Source { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public TemplateId Template { get; set; }

        public SkillId Skill { get; set; }

        public ActionEndKind Kind { get; set; }

        public uint CorrelationId { get; set; }
    }
}
