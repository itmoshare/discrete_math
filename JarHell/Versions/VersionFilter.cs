using System;
using JetBrains.Annotations;

namespace JarHell.Versions
{
    public class VersionFilter : IEquatable<VersionFilter>
    {
        public VersionFilter(int? major, int? minor, int? patch)
        {
            EnsureValid(major, minor, patch);

            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int? Major { get; }

        public int? Minor { get; }

        public int? Patch { get; }

        [AssertionMethod]
        private static void EnsureValid(int? major, int? minor, int? patch)
        {
            if (major == null && (minor != null || patch != null))
                throw new ArgumentException("Invalid version filter");

            if (minor == null && patch != null) throw new ArgumentException("Invalid version filter");
        }

        public Version GetMinVersion()
        {
            return new Version(
                Major ?? 0,
                Minor ?? 0,
                Patch ?? 0);
        }

        public Version GetMaxVersion()
        {
            return new Version(
                Major ?? int.MaxValue,
                Minor ?? int.MaxValue,
                Patch ?? int.MaxValue);
        }

        public override string ToString()
        {
            var major = Major?.ToString() ?? "*";
            var minor = Minor?.ToString() ?? "*";
            var patch = Patch?.ToString() ?? "*";
            return $"{major}.{minor}.{patch}";
        }

        public bool Equals(VersionFilter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VersionFilter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major.GetHashCode();
                hashCode = (hashCode * 397) ^ Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ Patch.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(VersionFilter left, VersionFilter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VersionFilter left, VersionFilter right)
        {
            return !Equals(left, right);
        }
    }
}