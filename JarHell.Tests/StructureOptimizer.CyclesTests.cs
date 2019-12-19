using System.Linq;
using JarHell.Optimizers.StructureOptimizer;
using JarHell.Packages;
using NUnit.Framework;

namespace JarHell.Tests
{
    public class StructureOptimizerCyclesTests
    {
        [Test]
        public void CollectCycles_WhenCycleInLocalPackages_ShouldOfferAllPossibleFixes()
        {
            var optimizer = StructureOptimizer.Create(new[]
                {
                    new PackageMeta("", true, new PackageInfo
                    {
                        Name = "A",
                        Dependencies = new[] {new Dependency {Name = "B"}}
                    }),
                    new PackageMeta("", true, new PackageInfo
                    {
                        Name = "B",
                        Dependencies = new[] {new Dependency {Name = "C"}}
                    }),
                    new PackageMeta("", true, new PackageInfo
                    {
                        Name = "C",
                        Dependencies = new[] {new Dependency {Name = "A"}}
                    })
                },
                null,
                "A");

            var actual = optimizer.CollectCycles().ToArray();

            Assert.That(actual, Has.Length.EqualTo(1));
            Assert.That(actual[0], Has.Length.EqualTo(3));
            Assert.That(actual[0], Has.Some.Matches<StructureOptimizer.CycleFixOffer>(
                x => x.SourceMeta.PackageInfo.Name == "A" && x.DependencyToRemove == "B"));
            Assert.That(actual[0], Has.Some.Matches<StructureOptimizer.CycleFixOffer>(
                x => x.SourceMeta.PackageInfo.Name == "B" && x.DependencyToRemove == "C"));
            Assert.That(actual[0], Has.Some.Matches<StructureOptimizer.CycleFixOffer>(
                x => x.SourceMeta.PackageInfo.Name == "C" && x.DependencyToRemove == "A"));
        }

        [Test]
        public void CollectCycles_WhenCycle_ShouldExcludeNonLocalPackages()
        {
            var optimizer = StructureOptimizer.Create(new[]
                {
                    new PackageMeta("", true, new PackageInfo
                    {
                        Name = "A",
                        Dependencies = new[] {new Dependency {Name = "B"}}
                    }),
                    new PackageMeta("", false, new PackageInfo
                    {
                        Name = "B",
                        Dependencies = new[] {new Dependency {Name = "C"}}
                    }),
                    new PackageMeta("", true, new PackageInfo
                    {
                        Name = "C",
                        Dependencies = new[] {new Dependency {Name = "A"}}
                    })
                },
                null,
                "A");

            var actual = optimizer.CollectCycles().ToArray();

            Assert.That(actual, Has.Length.EqualTo(1));
            Assert.That(actual[0], Has.Length.EqualTo(2));
            Assert.That(actual[0], Has.Some.Matches<StructureOptimizer.CycleFixOffer>(
                x => x.SourceMeta.PackageInfo.Name == "A" && x.DependencyToRemove == "B"));
            Assert.That(actual[0], Has.Some.Matches<StructureOptimizer.CycleFixOffer>(
                x => x.SourceMeta.PackageInfo.Name == "C" && x.DependencyToRemove == "A"));
        }
    }
}