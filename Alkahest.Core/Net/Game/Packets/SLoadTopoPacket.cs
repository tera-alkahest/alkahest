using Alkahest.Core.Net.Game.Serialization;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LOAD_TOPO")]
    public sealed class SLoadTopoPacket : SerializablePacket
    {
        public uint HuntingZoneId { get; set; }

        public Vector3 Position { get; set; }

        public bool IsQuick { get; set; }
    }
}
