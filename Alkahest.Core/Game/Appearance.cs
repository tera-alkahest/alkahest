using System;

namespace Alkahest.Core.Game
{
    public readonly struct Appearance : IEquatable<Appearance>
    {
        public static Appearance Default => default;

        public ulong Raw { get; }

        public bool IsDefault => this == Default;

        public byte Unknown => (byte)Bits.Extract(Raw, 0, 8);

        public byte SkinColor => (byte)Bits.Extract(Raw, 8, 8);

        public byte FaceStyle => (byte)Bits.Extract(Raw, 16, 8);

        public byte FaceDecals => (byte)Bits.Extract(Raw, 24, 8);

        public byte HairStyle => (byte)Bits.Extract(Raw, 32, 8);

        public byte HairColor => (byte)Bits.Extract(Raw, 40, 8);

        public byte Voice => (byte)Bits.Extract(Raw, 48, 8);

        public byte Tattoos => (byte)Bits.Extract(Raw, 56, 8);

        public Appearance(ulong raw)
        {
            Raw = raw;
        }

        public static Appearance FromValues(byte unknown, byte skinColor, byte faceStyle,
            byte faceDecals, byte hairStyle, byte hairColor, byte voice, byte tattoos)
        {
            return new Appearance(Bits.Compose(
                ((ulong)unknown, 0, 8),
                (skinColor, 8, 8),
                (faceStyle, 16, 8),
                (faceDecals, 24, 8),
                (hairStyle, 32, 8),
                (hairColor, 40, 8),
                (voice, 48, 8),
                (tattoos, 56, 8)));
        }

        public bool Equals(Appearance other)
        {
            return Raw == other.Raw;
        }

        public override bool Equals(object obj)
        {
            return obj is Appearance a ? Equals(a) : false;
        }

        public override int GetHashCode()
        {
            return Raw.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Raw: {Raw}, SkinColor: {SkinColor}, FaceStyle: {FaceStyle}, FaceDecals: {FaceDecals}, " +
                $"HairStyle: {HairStyle}, HairColor: {HairColor}, Voice: {Voice}, Tattoos: {Tattoos}]";
        }

        public static bool operator ==(Appearance left, Appearance right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Appearance left, Appearance right)
        {
            return !left.Equals(right);
        }
    }
}
