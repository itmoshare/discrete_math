using System.Collections.Generic;
using System.Linq;
using JarHell.Packages;

namespace JarHell.Optimizers
{
    public class VersionsSynchronizer
    {
        public IEnumerable<SynchronizeVersionOffer> GetSynchronizeOffers(PackageMeta[] localPackages)
        {
            var packagesWithDependencies = localPackages.SelectMany(
                    x => x.PackageInfo.Dependencies,
                    (package, dependency) => new { sourcePackage = package, dependency })
                .ToArray();

            foreach (var groupedByDependencyName in packagesWithDependencies.GroupBy(x => x.dependency.Name))
            {
                var highestMinVersion = groupedByDependencyName.Select(x => new
                    {
                        minVersion = x.dependency.Version.ToRange().lower,
                        versionFilter = x.dependency.Version.LowerBoundToFilter()
                    })
                    .OrderByDescending(x => x.minVersion)
                    .First();
                var highestMaxVersion = groupedByDependencyName.Select(x => new
                    {
                        maxVersion = x.dependency.Version.ToRange().upper,
                        versionFilter = x.dependency.Version.UpperBoundToFilter()
                    })
                    .OrderByDescending(x => x.maxVersion)
                    .First();

                foreach (var package in groupedByDependencyName)
                {
                    if (package.dependency.Version.ToRange().lower != highestMinVersion.minVersion
                        || package.dependency.Version.ToRange().upper != highestMaxVersion.maxVersion)
                    {
                        yield return new SynchronizeVersionOffer(
                            "Inconsistent versions targets were found.",
                            package.sourcePackage,
                            package.dependency.Name,
                            highestMinVersion.minVersion != highestMaxVersion.maxVersion
                                ? new VersionTarget(highestMinVersion.versionFilter, highestMaxVersion.versionFilter)
                                : new VersionTarget(highestMinVersion.versionFilter));
                    }
                }
            }
        }

        public class SynchronizeVersionOffer
        {
            public SynchronizeVersionOffer(
                string description,
                PackageMeta sourceMeta,
                string dependencyToUpdate,
                VersionTarget newDependencyTarget)
            {
                Description = description;
                SourceMeta = sourceMeta;
                DependencyToUpdate = dependencyToUpdate;
                NewDependencyTarget = newDependencyTarget;
            }

            public string Description { get; }

            public PackageMeta SourceMeta { get; }

            public string DependencyToUpdate { get; }

            public VersionTarget NewDependencyTarget { get; }
        }
    }
}