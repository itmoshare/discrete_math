using System;
using JetBrains.Annotations;

namespace JarHell.Versions
{
    public class VersionFilter
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

        public override string ToString()
        {
            var major = Major.ToString() ?? "*";
            var minor = Minor.ToString() ?? "*";
            var patch = Patch.ToString() ?? "*";
            return $"{major}.{minor}.{patch}";
        }
    }
}