using System;

namespace Alkahest.Core.Data
{
    public struct SkillId : IEquatable<SkillId>
    {
        public readonly uint Raw;

        public uint Skill => Bits.Extract(Raw, 0, 25);

        public SkillFlags Flags => (SkillFlags)Bits.Extract(Raw, 26, 31);

        public uint Category => Skill / 10000;

        public uint Level => Skill / 100 % 100;

        public uint Type => Skill % 100;

        public SkillId(uint raw)
        {
            Raw = raw;
        }

        public SkillId(uint category, uint level, uint type)
        {
            Raw = Bits.Insert(0, (category * 10000 + level * 100 + type), 0, 23);
            Raw |= Bits.Insert(0, (uint)SkillFlags.Unknown1, 24, 31);
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
            return $"[Skill: {Skill}, Flags: {Flags}]";
        }

        public static bool operator ==(SkillId a, SkillId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SkillId a, SkillId b)
        {
            return !(a == b);
        }
    }
}
