using System;

namespace Alkahest.Core.Plugins
{
    public sealed class PacketFilter
    {
        // Reserve some low/high order values for Alkahest itself.

        public static long MinOrder => long.MinValue + uint.MaxValue;

        public static long MaxOrder => long.MaxValue - uint.MaxValue;

        public long Order { get; private set; }

        public bool? Forged { get; private set; }

        public bool? Modified { get; private set; }

        public bool? Silenced { get; private set; }

        public Predicate<Packet> Function { get; private set; }

        readonly bool _internal;

        // Exposed to core plugins through InternalsVisibleToAttribute.
        internal PacketFilter(long order)
            : this()
        {
            Order = order;
            _internal = true;
        }

        public PacketFilter(long order = 0, bool? forged = false, bool? modified = null,
            bool? silenced = false, Predicate<Packet> function = null)
        {
            if (order < MinOrder || order > MaxOrder)
                throw new ArgumentOutOfRangeException(nameof(order));

            Order = order;
            Forged = forged;
            Modified = modified;
            Silenced = silenced;
            Function = function;
        }

        private PacketFilter(PacketFilter other)
        {
            Order = other.Order;
            Forged = other.Forged;
            Modified = other.Modified;
            Silenced = other.Silenced;
            Function = other.Function;
        }

        public PacketFilter WithOrder(long order)
        {
            if (!_internal && (order < MinOrder || order > MaxOrder))
                throw new ArgumentOutOfRangeException(nameof(order));

            return new PacketFilter(this)
            {
                Order = order,
            };
        }

        public PacketFilter WithForged(bool? forged)
        {
            return new PacketFilter(this)
            {
                Forged = forged,
            };
        }

        public PacketFilter WithModified(bool? modified)
        {
            return new PacketFilter(this)
            {
                Modified = modified,
            };
        }

        public PacketFilter WithSilenced(bool? silenced)
        {
            return new PacketFilter(this)
            {
                Silenced = silenced,
            };
        }

        public PacketFilter WithFunction(Predicate<Packet> function)
        {
            return new PacketFilter(this)
            {
                Function = function,
            };
        }
    }
}
