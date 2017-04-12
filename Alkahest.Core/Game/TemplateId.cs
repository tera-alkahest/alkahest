using System;

namespace Alkahest.Core.Game
{
    public struct TemplateId : IEquatable<TemplateId>
    {
        public static readonly TemplateId Zero = new TemplateId(0);

        public readonly uint Raw;

        public Race Race => (Race)((Raw - 100) / 200 % 50);

        public Gender Gender => (Gender)(Raw / 100 % 2);

        public Class Class => (Class)(Raw % 100 - 1);

        public TemplateId(uint raw)
        {
            Raw = raw;
        }

        public static TemplateId FromValues(Race race, Gender gender, Class @class)
        {
            return new TemplateId(10200 + 200 * (uint)race -
                100 * (uint)gender + (uint)@class + 1);
        }

        public bool Equals(TemplateId other)
        {
            return Raw == other.Raw;
        }

        public override bool Equals(object obj)
        {
            return obj is TemplateId s ? Equals(s) : false;
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Raw: {Raw}, Race: {Race}, Gender: {Gender}, Class: {Class}]";
        }

        public static bool operator ==(TemplateId a, TemplateId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(TemplateId a, TemplateId b)
        {
            return !a.Equals(b);
        }
    }
}
