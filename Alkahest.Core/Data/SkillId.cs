using System;

namespace Alkahest.Core.Data
{
    public struct SkillId : IEquatable<SkillId>
    {
        public readonly uint Raw;

        public uint Skill => Bits.Extract(Raw, 0, 25);

        public SkillFlags Flags => (SkillFlags)Bits.Extract(Raw, 26, 31);

        public uint Group => Skill / 10000;

        public uint Level => Skill / 100 % 100;

        public uint Type => Skill % 100;

        public SkillId(uint raw)
        {
            Raw = raw;
        }

        public static SkillId FromSkill(uint skill)
        {
            return new SkillId(Bits.Insert(0, skill, 0, 25) |
                Bits.Insert(0, (uint)SkillFlags.Unknown1, 26, 31));
        }

        public static SkillId FromValues(uint group, uint level, uint type)
        {
            return new SkillId(Bits.Insert(0, (group * 10000 + level * 100 + type), 0, 25) |
                Bits.Insert(0, (uint)SkillFlags.Unknown1, 26, 31));
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
            return $"[Raw: {Raw}, Skill: {Skill}, Flags: {Flags}]";
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
