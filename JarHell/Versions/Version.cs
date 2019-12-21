using System;
using Newtonsoft.Json;

namespace JarHell.Versions
{
    [JsonConverter(typeof(VersionConverter))]
    public class Version : IComparable<Version>
    {
        protected bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }
        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; }

        public int Minor { get; }

        public int Patch { get; }

        public int CompareTo(Version other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0) return majorComparison;

            var minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0) return minorComparison;

            return Patch.CompareTo(other.Patch);
        }

        public static bool operator ==(Version v1, Version v2)
        {
            return v1?.CompareTo(v2) == 0;
        }

        public static bool operator !=(Version v1, Version v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(Version v1, Version v2)
        {
            return v1?.CompareTo(v2) < 0;
        }

        public static bool operator <=(Version v1, Version v2)
        {
            var cmp = v1?.CompareTo(v2);
            return cmp < 0 || cmp == 0;
        }

        public static bool operator >(Version v1, Version v2)
        {
            return v1?.CompareTo(v2) > 0;
        }

        public static bool operator >=(Version v1, Version v2)
        {
            var cmp = v1?.CompareTo(v2);
            return cmp > 0 || cmp == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Version) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }
}