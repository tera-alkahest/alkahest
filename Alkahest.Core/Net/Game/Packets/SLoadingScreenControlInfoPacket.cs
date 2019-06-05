using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SLoadingScreenControlInfoPacket : Packet
    {
        const string Name = "S_LOADING_SCREEN_CONTROL_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoadingScreenControlInfoPacket();
        }

        [PacketField]
        public bool EnableCustom { get; set; }
    }
}
