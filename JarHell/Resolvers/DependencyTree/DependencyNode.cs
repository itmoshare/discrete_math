using System.Collections.Generic;
using System.Linq;
using JarHell.Versions;
using JetBrains.Annotations;

namespace JarHell.Resolvers.DependencyTree
{
    internal class DependencyNode
    {
        [CanBeNull]
        public string PackageName { get; set; }

        [CanBeNull]
        public Version PackageVersion { get; set; }

        public List<DependencyNode> Dependencies { get; } = new List<DependencyNode>();

        public void AttachDependency(DependencyNode dependencyNode)
        {
            Dependencies.Add(dependencyNode);
        }

        public IEnumerable<DependencyNode> EnumerateDfs() => EnumerateDfsImpl(this);

        private static IEnumerable<DependencyNode> EnumerateDfsImpl(DependencyNode node)
        {
            foreach (var dependency in node.Dependencies.SelectMany(EnumerateDfsImpl))
            {
                yield return dependency;
            }

            yield return node;
        }

        public override string ToString()
        {
            return $"{PackageName} {PackageVersion}: [{string.Concat(Dependencies.Select(x => $"{x.PackageName} {x.PackageVersion},"))}]";
        }
    }
}