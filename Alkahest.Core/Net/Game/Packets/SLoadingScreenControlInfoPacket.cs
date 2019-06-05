using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LOADING_SCREEN_CONTROL_INFO")]
    public sealed class SLoadingScreenControlInfoPacket : SerializablePacket
    {
        public bool EnableCustom { get; set; }
    }
}
