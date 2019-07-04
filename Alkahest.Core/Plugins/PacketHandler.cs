using System;

namespace Alkahest.Core.Plugins
{
    public sealed class PacketHandler
    {
        public bool IsRaw { get; }

        public string Name { get; }

        public ushort? Code { get; }

        public Delegate Handler { get; }

        public PacketFilter Filter { get; }

        internal PacketHandler(bool raw, string name, ushort? code, Delegate handler,
            PacketFilter filter)
        {
            IsRaw = raw;
            Name = name;
            Code = code;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Filter = filter ?? new PacketFilter();
        }
    }
}
