using JarHell.Versions;
using Newtonsoft.Json;

namespace JarHell.Packages
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PackageInfo
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public Version Version { get; set; }

        [JsonProperty]
        public Dependency[] Dependencies { get; set; }
    }
}