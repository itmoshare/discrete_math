using System;
using System.Collections.Generic;
using System.Linq;
using JarHell.Packages;

namespace JarHell.Core
{
    public class PackagesResolver
    {
        public ResolvedPackage Resolve(
            PackageRepository localRepository,
            PackageRepository packageRepository,
            string target)
        {
            var targetPackage = packageRepository.FindPackages(target).Last();
            return ResolveInner(
                targetPackage,
                localRepository,
                packageRepository,
                new Dictionary<string, List<ResolvedPackage>>());
        }

        private static ResolvedPackage ResolveInner(
            PackageMeta package,
            PackageRepository localRepository,
            PackageRepository repository,
            IDictionary<string, List<ResolvedPackage>> resolved)
        {
            var resolvedPackage = new ResolvedPackage(package);
            if (resolved.TryGetValue(package.PackageInfo.Name, out var alreadyResolved))
            {
                alreadyResolved.Add(resolvedPackage);
            }

            foreach (var packageInfoDependency in package.PackageInfo.Dependencies)
            {
                var (lower, upper) = packageInfoDependency.Version.ToRange();

                if (resolved.TryGetValue(packageInfoDependency.Name, out var packages))
                {
                    var suitableResolvedPackage = packages.FirstOrDefault(x =>
                        lower <= x.PackageMeta.PackageInfo.Version
                        && x.PackageMeta.PackageInfo.Version <= upper);
                    if (suitableResolvedPackage != null)
                    {
                        resolvedPackage.ResolvedDependencies.Add(suitableResolvedPackage);
                        continue;
                    }
                }

                var suitableLocalPackages = localRepository.FindPackages(packageInfoDependency.Name, lower, upper);
                if (suitableLocalPackages.Any())
                {
                    var newResolvedPackage = ResolveInner(
                        suitableLocalPackages.First(),
                        localRepository,
                        repository,
                        resolved);
                    resolvedPackage.ResolvedDependencies.Add(newResolvedPackage);
                    continue;
                }

                var suitableRepositoryPackages = repository.FindPackages(packageInfoDependency.Name, lower, upper);
                if (suitableRepositoryPackages.Any())
                {
                    var newResolvedPackage = ResolveInner(
                        suitableRepositoryPackages.First(),
                        localRepository,
                        repository,
                        resolved);
                    resolvedPackage.ResolvedDependencies.Add(newResolvedPackage);
                    continue;
                }

                throw new InvalidOperationException($"Dependency {packageInfoDependency.Name} of package {package.PackageInfo.Name} can't be found");
            }

            return resolvedPackage;
        }
    }
}