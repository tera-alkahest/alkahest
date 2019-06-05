using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_CHAT")]
    public sealed class SChatPacket : SerializablePacket
    {
        public string SenderName { get; set; }

        public string Message { get; set; }

        public ChatChannel Channel { get; set; }

        public GameId Source { get; set; }

        public bool IsWorldEventTarget { get; set; }

        public bool IsGameMaster { get; set; }

        public bool IsFounder { get; set; }
    }
}
