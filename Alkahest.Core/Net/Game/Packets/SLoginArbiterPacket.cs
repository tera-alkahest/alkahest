using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LOGIN_ARBITER")]
    public sealed class SLoginArbiterPacket : SerializablePacket
    {
        public bool IsSuccessful { get; set; }

        public bool IsInQueue { get; set; }

        public uint Status { get; set; }

        public int Unknown1 { get; set; }

        public ClientRegion Region { get; set; }

        public bool DisablePlayerVersusPlayer { get; set; }

        public short Unknown2 { get; set; }

        public short Unknown3 { get; set; }
    }
}
