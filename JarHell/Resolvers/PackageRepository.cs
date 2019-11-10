using System;
using System.Collections.Generic;
using System.Linq;
using JarHell.Packages;
using JarHell.Versions;
using Version = JarHell.Versions.Version;

namespace JarHell.Resolvers
{
    public class PackageRepository
    {
        private readonly Dictionary<string, List<PackageInfo>> _packagesByName;
        private readonly Dictionary<(string, Version), PackageInfo> _packagesByNameAndVersion;

        public PackageRepository()
        {
            _packagesByName = new Dictionary<string, List<PackageInfo>>();
            _packagesByNameAndVersion = new Dictionary<(string, Version), PackageInfo>();
        }

        public void AddPackage(PackageInfo packageInfo)
        {
            if (_packagesByNameAndVersion.ContainsKey((packageInfo.Name, packageInfo.Version)))
            {
                throw new ArgumentException($"Package {packageInfo.Name} {packageInfo.Version} was registered twice");
            }

            if (!_packagesByName.TryAdd(packageInfo.Name, new List<PackageInfo> { packageInfo }))
            {
                _packagesByName[packageInfo.Name].Add(packageInfo);
            }

            _packagesByNameAndVersion.Add((packageInfo.Name, packageInfo.Version), packageInfo);
        }

        public PackageInfo[] FindPackages(string name, Version lower, Version upper)
        {
            if (_packagesByName.TryGetValue(name, out var packages))
            {
                return packages
                    .Where(package => lower < package.Version && package.Version < upper)
                    .ToArray();
            }

            return Array.Empty<PackageInfo>();
        }
    }
}