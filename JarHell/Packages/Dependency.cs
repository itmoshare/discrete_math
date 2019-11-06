using Newtonsoft.Json;

namespace JarHell.Packages
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Dependency
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public VersionTarget Version { get; set; }
    }
}