using System;

namespace Alkahest.Core.Net.Game
{
    public struct PacketEventArgs
    {
        public GameClient Client { get; }

        public Direction Direction { get; }

        public ushort Code { get; }

        public Memory<byte> Payload { get; set; }

        public bool Silence { get; set; }

        internal PacketEventArgs(GameClient client, Direction direction, ushort code,
            Memory<byte> payload)
        {
            Client = client;
            Direction = direction;
            Code = code;
            Payload = payload;
            Silence = false;
        }
    }
}
