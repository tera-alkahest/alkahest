using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_ABNORMALITY_DAMAGE_ABSORB")]
    public sealed class SAbnormalityDamageAbsorbPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public uint Damage { get; set; }

        public int Unknown1 { get; set; }
    }
}
