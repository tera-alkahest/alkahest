using Alkahest.Core.Net.Game;
using System;

namespace Alkahest.Core.Plugins
{
    public readonly struct Packet
    {
        public GameClient Client { get; }

        public Direction Direction { get; }

        public ushort Code { get; }

        public Memory<byte> Payload { get; }

        internal Packet(GameClient client, Direction direction, ushort code,
            Memory<byte> payload)
        {
            Client = client;
            Direction = direction;
            Code = code;
            Payload = payload;
        }
    }
}
