using Newtonsoft.Json;

namespace JarHell.Packages
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Dependency
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public VersionTarget Version { get; set; }

        public override string ToString()
        {
            return $"{Name} {Version}";
        }
    }
}