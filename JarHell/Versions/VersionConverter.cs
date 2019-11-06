using System;
using Newtonsoft.Json;

namespace JarHell.Versions
{
    public class VersionConverter : JsonConverter<Version>
    {
        public override void WriteJson(
            JsonWriter writer,
            Version value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Version ReadJson(
            JsonReader reader,
            Type objectType,
            Version existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = (string) reader.Value;
            var versionComponents = value.Split('.');
            return versionComponents.Length switch
            {
                3 => new Version(
                    int.Parse(versionComponents[0]),
                    int.Parse(versionComponents[1]),
                    int.Parse(versionComponents[2])),
                _ => throw new FormatException()
            };
        }
    }
}