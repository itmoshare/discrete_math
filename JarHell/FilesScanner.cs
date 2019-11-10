using System.IO;
using System.Linq;
using JarHell.Packages;
using Newtonsoft.Json;

namespace JarHell
{
    public static class FilesScanner
    {
        public static PackageInfo[] GetAllPackages(string[] paths, bool recursive = false)
        {
            return paths
                .SelectMany(path => Directory.EnumerateFiles(
                    path,
                    "*.package",
                    recursive
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly))
                .Select(path => JsonConvert.DeserializeObject<PackageInfo>(File.ReadAllText(path)))
                .ToArray();
        }

        public static PackageInfo[] GetPackages(string[] paths)
        {
            return paths
                .Select(path => JsonConvert.DeserializeObject<PackageInfo>(File.ReadAllText(path)))
                .ToArray();
        }
    }
}