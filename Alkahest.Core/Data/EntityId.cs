using System;

namespace Alkahest.Core.Data
{
    public struct EntityId : IEquatable<EntityId>
    {
        public readonly ulong Raw;

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
            return Raw.ToString("X16");
        }

        public static bool operator ==(EntityId a, EntityId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(EntityId a, EntityId b)
        {
            return !(a == b);
        }
    }
}
