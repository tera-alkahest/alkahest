using System;

namespace Alkahest.Core.Data
{
    public struct EntityId : IEquatable<EntityId>
    {
        public static readonly EntityId Zero = new EntityId(0);

        public readonly ulong Raw;

        public bool IsZero => this == Zero;

        public uint Id => (uint)Bits.Extract(Raw, 0, 31);

        public EntityFlags Flags => (EntityFlags)Bits.Extract(Raw, 32, 63);

        public EntityId(ulong raw)
        {
            Raw = raw;
        }

        public bool Equals(EntityId other)
        {
            return Raw == other.Raw;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityId e ? Equals(e) : false;
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Raw: {Raw:X16}, Id: {Id}, Flags: {Flags}]";
        }

        public static bool operator ==(EntityId a, EntityId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(EntityId a, EntityId b)
        {
            return !a.Equals(b);
        }
    }
}
