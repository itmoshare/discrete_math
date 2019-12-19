using System;
using System.Collections.Generic;
using System.Linq;
using JarHell.Packages;

namespace JarHell.Optimizers.StructureOptimizer
{
    public partial class StructureOptimizer
    {
        private static Node BuildTree(
            PackageMeta package,
            IDictionary<string, PackageMeta> localPackages,
            PackageRepository repository,
            HashSet<string> visited)
        {
            var node = new Node(package);
            visited.Add(package.PackageInfo.Name);
            foreach (var dependency in package.PackageInfo.Dependencies)
            {
                var depPackage = localPackages.ContainsKey(dependency.Name)
                    ? localPackages[dependency.Name]
                    : repository.FindPackages(
                            dependency.Name,
                            dependency.Version.ToRange().lower,
                            dependency.Version.ToRange().upper)
                        .LastOrDefault();

                if (depPackage == null)
                {
                    throw new ArgumentException($"Package {dependency} wasn't found");
                }

                node.Children.Add(visited.Contains(dependency.Name)
                    ? new Node(depPackage)
                    : BuildTree(depPackage, localPackages, repository, visited));
            }

            visited.Remove(package.PackageInfo.Name);
            return node;
        }

        private class Node
        {
            public Node(PackageMeta packageMeta)
            {
                PackageMeta = packageMeta;
            }

            public PackageMeta PackageMeta { get; }

            public List<Node> Children { get; } = new List<Node>();
        }
    }
}