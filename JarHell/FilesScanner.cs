using System.Collections.Generic;
using System.IO;
using System.Linq;
using JarHell.Packages;
using Newtonsoft.Json;

namespace JarHell
{
    public static class FilesScanner
    {
        public static PackageMeta[] GetAllPackages(string[] paths, bool recursive = false)
        {
            return paths
                .SelectMany(path =>
                {
                    if (!Directory.Exists(path))
                    {
                        return Enumerable.Empty<string>();
                    }

                    return Directory.EnumerateFiles(
                        path,
                        "*.package",
                        recursive
                            ? SearchOption.AllDirectories
                            : SearchOption.TopDirectoryOnly);
                })
                .Select(path => new PackageMeta(path, JsonConvert.DeserializeObject<PackageInfo>(File.ReadAllText(path))))
                .ToArray();
        }
    }
}