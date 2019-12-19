using System;
using System.Collections.Generic;
using System.Linq;
using Version = JarHell.Versions.Version;

namespace JarHell.Packages
{
    public class PackageRepository
    {
        private readonly Dictionary<string, List<PackageMeta>> _packagesByName;
        private readonly Dictionary<(string, Version), PackageMeta> _packagesByNameAndVersion;

        public PackageRepository()
        {
            _packagesByName = new Dictionary<string, List<PackageMeta>>();
            _packagesByNameAndVersion = new Dictionary<(string, Version), PackageMeta>();
        }

        public void AddPackage(PackageMeta packageMeta)
        {
            if (_packagesByNameAndVersion.ContainsKey((packageMeta.PackageInfo.Name, packageMeta.PackageInfo.Version)))
            {
                throw new ArgumentException($"Package {packageMeta.PackageInfo.Name} {packageMeta.PackageInfo.Version} was registered twice");
            }

            if (!_packagesByName.TryAdd(packageMeta.PackageInfo.Name, new List<PackageMeta> { packageMeta }))
            {
                _packagesByName[packageMeta.PackageInfo.Name].Add(packageMeta);
            }

            _packagesByNameAndVersion.Add((packageMeta.PackageInfo.Name, packageMeta.PackageInfo.Version), packageMeta);
        }

        public PackageMeta[] FindPackages(string name, Version lower, Version upper)
        {
            if (_packagesByName.TryGetValue(name, out var packages))
            {
                return packages
                    .Where(package => lower < package.PackageInfo.Version && package.PackageInfo.Version < upper)
                    .OrderBy(x => x.PackageInfo.Version)
                    .ToArray();
            }

            return Array.Empty<PackageMeta>();
        }
    }
}