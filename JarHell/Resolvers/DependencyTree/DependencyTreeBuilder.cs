using System;
using System.Linq;
using JarHell.Packages;

namespace JarHell.Resolvers.DependencyTree
{
    internal static class DependencyTreeBuilder
    {
        public static DependencyNode Build(PackageRepository packageRepository, PackageInfo[] rootPackages)
        {
            var fakeRoot = new DependencyNode();
            foreach (var rootPackage in rootPackages)
            {
                fakeRoot.AttachDependency(Build(packageRepository, rootPackage));
            }

            return fakeRoot;
        }

        private static DependencyNode Build(PackageRepository packageRepository, PackageInfo rootPackage)
        {
            var node = new DependencyNode
            {
                PackageName = rootPackage.Name,
                PackageVersion = rootPackage.Version
            };

            foreach (var packageDependency in rootPackage.Dependencies)
            {
                var (minVersion, maxVersion) = packageDependency.Version.ToRange();
                var suitablePackages = packageRepository.FindPackages(packageDependency.Name, minVersion, maxVersion);
                if (!suitablePackages.Any())
                {
                    throw new ArgumentException($"Package for dependency {packageDependency} wasn't found.");
                }

                if (suitablePackages.Length > 1)
                {
                    Console.WriteLine($"Multiple packages for dependency {packageDependency} were found. The newest will be used.");
                }

                var package = suitablePackages.OrderByDescending(x => x.Version).FirstOrDefault();
                node.AttachDependency(Build(packageRepository, package));
            }

            return node;
        }
    }
}