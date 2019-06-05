using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_ACCEPT_CONTRACT")]
    public sealed class CAcceptContractPacket : SerializablePacket
    {
        public int Unknown1 { get; set; }

        public uint QuestId { get; set; }
    }
}
