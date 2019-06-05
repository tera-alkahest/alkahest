using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_GUARD_PK_POLICY")]
    public sealed class SGuardPKPolicyPacket : SerializablePacket
    {
        public byte Unknown1 { get; set; }
    }
}
