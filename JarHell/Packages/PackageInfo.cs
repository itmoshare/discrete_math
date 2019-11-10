using JarHell.Versions;
using Newtonsoft.Json;

namespace JarHell.Packages
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PackageInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("dependencies")]
        public Dependency[] Dependencies { get; set; }
    }
}