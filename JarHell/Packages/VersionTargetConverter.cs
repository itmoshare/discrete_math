using System;
using JarHell.Versions;
using Newtonsoft.Json;

namespace JarHell.Packages
{
    public class VersionTargetConverter : JsonConverter<VersionTarget>
    {
        public override void WriteJson(
            JsonWriter writer,
            VersionTarget value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override VersionTarget ReadJson(
            JsonReader reader,
            Type objectType,
            VersionTarget existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = (string) reader.Value;
            var rangeComponents = value.Split('-');
            return rangeComponents.Length switch
            {
                1 => new VersionTarget(Parse(rangeComponents[0])),

                2 => new VersionTarget(Parse(rangeComponents[0]), Parse(rangeComponents[1])),

                _=> throw new FormatException()
            };
        }

        private static VersionFilter Parse(string data)
        {
            var components = data.Split('.');
            return components.Length switch
            {
                3 => new VersionFilter(
                    components[0] != "*"
                        ? int.Parse(components[0])
                        : (int?) null,
                    components[1] != "*"
                        ? int.Parse(components[1])
                        : (int?) null,
                    components[2] != "*"
                        ? int.Parse(components[2])
                        : (int?) null),
                _ => throw new FormatException()
            };
        }
    }
}