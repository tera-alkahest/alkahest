using System;

namespace Alkahest.Core.Data
{
    public struct ModelId : IEquatable<ModelId>
    {
        public readonly uint Raw;

        public Race Race => (Race)((Raw - 100) / 200 % 50);

        public Gender Gender => (Gender)(Raw / 100 % 2);

        public Class Class => (Class)(Raw % 100 - 1);

        public ModelId(uint raw)
        {
            Raw = raw;
        }

        public static ModelId FromValues(Race race, Gender gender, Class @class)
        {
            return new ModelId(10200 + 200 * (uint)race -
                100 * (uint)gender + (uint)@class + 1);
        }

        public bool Equals(ModelId other)
        {
            return Raw == other.Raw;
        }

        public override bool Equals(object obj)
        {
            return obj is ModelId s ? Equals(s) : false;
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Raw: {Raw}, Race: {Race}, Gender: {Gender}, Class: {Class}]";
        }

        public static bool operator ==(ModelId a, ModelId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ModelId a, ModelId b)
        {
            return !a.Equals(b);
        }
    }
}
