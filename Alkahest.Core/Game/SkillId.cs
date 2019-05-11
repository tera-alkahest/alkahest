using System;

namespace Alkahest.Core.Game
{
    public struct SkillId : IEquatable<SkillId>
    {
        public static readonly SkillId Zero = new SkillId();

        const long LocalSkillBase = 0x4000000;

        public readonly ulong Raw;

        public bool IsZero => this == Zero;

        public ulong Skill => Raw - LocalSkillBase;

        public ulong Group => Skill / 10000;

        public ulong Level => Skill / 100 % 100;

        public ulong Type => Skill % 100;

        public SkillId(ulong raw)
        {
            Raw = raw;
        }

        public static SkillId FromSkill(ulong skill)
        {
            return new SkillId(skill + LocalSkillBase);
        }

        public static SkillId FromValues(ulong group, ulong level, ulong type)
        {
            return FromSkill(group * 10000 + level * 100 + type);
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
            return $"[Raw: {Raw}, Skill: {Skill}, Group: {Group}, Level: {Level}, Type: {Type}]";
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
