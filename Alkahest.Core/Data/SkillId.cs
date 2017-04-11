using System;

namespace Alkahest.Core.Data
{
    public struct SkillId : IEquatable<SkillId>
    {
        public static readonly SkillId Zero = new SkillId(0);

        const int LocalSkillBase = 0x4000000;

        public readonly uint Raw;

        public uint Skill => Raw - LocalSkillBase;

        public uint Group => Skill / 10000;

        public uint Level => Skill / 100 % 100;

        public uint Type => Skill % 100;

        public SkillId(uint raw)
        {
            Raw = raw;
        }

        public static SkillId FromSkill(uint skill)
        {
            return new SkillId(skill + LocalSkillBase);
        }

        public static SkillId FromValues(uint group, uint level, uint type)
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
