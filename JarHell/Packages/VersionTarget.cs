using System;
using JarHell.Versions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Version = JarHell.Versions.Version;

namespace JarHell.Packages
{
    [JsonConverter(typeof(VersionTargetConverter))]
    public class VersionTarget
    {
        public VersionTarget(VersionFilter targetVersion)
        {
            TargetVersion = targetVersion;
        }

        public VersionTarget(VersionFilter lower, VersionFilter upper)
        {
            TargetRange = new Range(lower, upper);
            EnsureValidRange(TargetRange);
        }

        [CanBeNull]
        public VersionFilter TargetVersion { get; }

        [CanBeNull]
        public Range TargetRange { get; }

        public (Version lower, Version upper) ToRange() => TargetVersion != null
            ? (TargetVersion.GetMinVersion(), TargetVersion.GetMaxVersion())
            : (TargetRange.Lower.GetMinVersion(), TargetRange.Upper.GetMaxVersion());

        public override string ToString()
        {
            return TargetVersion?.ToString() ?? TargetRange?.ToString();
        }

        [AssertionMethod]
        private static void EnsureValidRange(Range range)
        {
            if (range.Lower.GetMinVersion() > range.Upper.GetMaxVersion())
            {
                throw new ArgumentException($"{range} is invalid version range");
            }
        }

        public class Range
        {
            public Range(VersionFilter lower, VersionFilter upper)
            {
                Lower = lower;
                Upper = upper;
            }

            public VersionFilter Lower { get; }

            public VersionFilter Upper { get; }

            public override string ToString()
            {
                return $"{Lower}-{Upper}";
            }
        }
    }
}