using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SWeakPointPacket : Packet
    {
        const string Name = "S_WEAK_POINT";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SWeakPointPacket();
        }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public uint RunemarksAdded { get; set; }

        [PacketField]
        public uint RunemarksRemoved { get; set; }

        [PacketField]
        public RunemarkEventKind EventKind { get; set; }

        [PacketField]
        public SkillId Skill { get; set; }
    }
}
