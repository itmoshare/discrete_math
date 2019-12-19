using System.Collections.Generic;
using System.Linq;
using JarHell.Packages;

namespace JarHell.Optimizers.StructureOptimizer
{
    public partial class StructureOptimizer
    {
        private readonly Node _node;

        private StructureOptimizer(Node node)
        {
            _node = node;
        }

        public static StructureOptimizer Create(
            PackageMeta[] localPackages,
            PackageRepository packageRepository,
            string target)
        {
            var targetPackage = localPackages.First(x => x.PackageInfo.Name == target);

            return new StructureOptimizer(BuildTree(
                targetPackage,
                localPackages.ToDictionary(x => x.PackageInfo.Name),
                packageRepository,
                new HashSet<string>()));
        }
    }
}