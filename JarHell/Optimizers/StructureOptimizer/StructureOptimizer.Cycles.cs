using System.Collections.Generic;
using System.Linq;
using JarHell.Packages;

namespace JarHell.Optimizers.StructureOptimizer
{
    public partial class StructureOptimizer
    {
        public IEnumerable<CycleFixOffer[]> CollectCycles()
        {
            return CollectCyclesInner(_node, new Dictionary<string, PackageMeta>());
        }

        private static IEnumerable<CycleFixOffer[]> CollectCyclesInner(Node root, Dictionary<string, PackageMeta> visited)
        {
            visited.Add(root.PackageMeta.PackageInfo.Name, root.PackageMeta);
            foreach (var child in root.Children)
            {
                if (visited.ContainsKey(child.PackageMeta.PackageInfo.Name))
                {
                    yield return visited
                        .Where(x => x.Value.Local)
                        .Select(x => new CycleFixOffer(
                            x.Value,
                            x.Value.PackageInfo.Dependencies.First(y => visited.ContainsKey(y.Name)).Name))
                        .ToArray();
                }
                else
                {
                    foreach (var cycle in CollectCyclesInner(child, visited))
                    {
                        yield return cycle;
                    }
                }
            }
            visited.Remove(root.PackageMeta.PackageInfo.Name);
        }

        public class CycleFixOffer
        {
            public CycleFixOffer(
                PackageMeta sourceMeta,
                string dependencyToRemove)
            {
                SourceMeta = sourceMeta;
                DependencyToRemove = dependencyToRemove;
            }

            public PackageMeta SourceMeta { get; }

            public string DependencyToRemove { get; }
        }
    }
}