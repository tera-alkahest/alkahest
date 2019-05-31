using System;

namespace Alkahest.Core.Game
{
    public struct SkillId : IEquatable<SkillId>
    {
        public static readonly SkillId Zero = default;

        public readonly ulong Raw;

        public bool IsZero => this == Zero;

        public bool HasHuntingZoneId => IsNpc && Kind == SkillKind.Action;

        public uint Id => (uint)Bits.Extract(Raw, 0, HasHuntingZoneId ? 16 : 28);

        public uint HuntingZoneId =>
            HasHuntingZoneId ? (uint)Bits.Extract(Raw, 16, 12) : throw new InvalidOperationException();

        public SkillKind Kind => (SkillKind)Bits.Extract(Raw, 28, 4);

        public bool IsNpc => Bits.Extract(Raw, 32, 1) == 1;

        public uint Unknown => (uint)Bits.Extract(Raw, 33, 31);

        public SkillId(ulong raw)
        {
            Raw = raw;
        }

        public static SkillId FromValues(uint id, SkillKind kind, bool isNpc, uint unknown)
        {
            return new SkillId(Bits.Compose(
                (id, 0, 28),
                ((ulong)kind.CheckValidity(nameof(kind)), 28, 4),
                (isNpc ? 1UL : 0, 32, 1),
                (unknown, 33, 31)));
        }

        public static SkillId FromValues(uint id, uint huntingZoneId, uint unknown)
        {
            return new SkillId(Bits.Compose(
                (id, 0, 16),
                (huntingZoneId, 16, 12),
                ((ulong)SkillKind.None, 28, 4),
                (0, 32, 1),
                (unknown, 33, 31)));
        }

        public bool Equals(SkillId other)
        {
            return Raw == other.Raw;
        }

        public override bool Equals(object obj)
        {
            return obj is SkillId s ? Equals(s) : false;
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Raw: {Raw}, Id: {Id}, HuntingZoneId: {(HasHuntingZoneId ? HuntingZoneId.ToString() : "N/A")}, " +
                $"Kind: {Kind}, IsNpc: {IsNpc}, Unknown: {Unknown}]";
        }

        public static bool operator ==(SkillId a, SkillId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SkillId a, SkillId b)
        {
            return !a.Equals(b);
        }
    }
}
