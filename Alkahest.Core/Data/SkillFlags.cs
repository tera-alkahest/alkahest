using System;

namespace Alkahest.Core.Data
{
    [Flags]
    public enum SkillFlags : byte
    {
        None = 0b00000000,
        Unknown1 = 0b00000001,
        Unknown2 = 0b00000010,
        Unknown3 = 0b00000100,
        Unknown4 = 0b00001000,
        Unknown5 = 0b00010000
    }
}
