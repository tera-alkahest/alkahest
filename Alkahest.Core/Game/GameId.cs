using System;

namespace Alkahest.Core.Game
{
    public struct GameId : IEquatable<GameId>
    {
        public static readonly GameId Zero = new GameId();

        public readonly ulong Raw;

        public bool IsZero => this == Zero;

        public uint Id => (uint)Bits.Extract(Raw, 0, 32);

        public EntityFlags Flags => (EntityFlags)Bits.Extract(Raw, 32, 32);

        public GameId(ulong raw)
        {
            Raw = raw;
        }

        public static GameId FromValues(uint id, EntityFlags flags)
        {
            return new GameId(Bits.Compose(
                (id, 0, 32),
                ((ulong)flags, 32, 32)));
        }

        public bool Equals(GameId other)
        {
            return Raw == other.Raw;
        }

        public override bool Equals(object obj)
        {
            return obj is GameId g ? Equals(g) : false;
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Raw: {Raw:X16}, Id: {Id}, Flags: {Flags}]";
        }

        public static bool operator ==(GameId left, GameId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GameId left, GameId right)
        {
            return !left.Equals(right);
        }
    }
}
