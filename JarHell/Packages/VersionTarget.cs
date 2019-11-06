using JarHell.Versions;
using JetBrains.Annotations;
using Newtonsoft.Json;

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
        }

        [CanBeNull]
        public VersionFilter TargetVersion { get; }

        [CanBeNull]
        public Range TargetRange { get; }

        public override string ToString()
        {
            return TargetVersion?.ToString() ?? TargetRange?.ToString();
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