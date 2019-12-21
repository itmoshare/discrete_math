using System.Collections.Generic;
using JarHell.Packages;

namespace JarHell.Core
{
    public class ResolvedPackage
    {
        public ResolvedPackage(PackageMeta packageMeta)
        {
            PackageMeta = packageMeta;
        }

        public PackageMeta PackageMeta { get; }

        public List<ResolvedPackage> ResolvedDependencies { get; } = new List<ResolvedPackage>();
    }
}