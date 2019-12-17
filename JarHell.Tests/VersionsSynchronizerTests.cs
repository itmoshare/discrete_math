using System.Linq;
using JarHell.Optimizers;
using JarHell.Packages;
using JarHell.Versions;
using NUnit.Framework;

namespace JarHell.Tests
{
    public class VersionsSynchronizerTests
    {
        [Test]
        public void GetSynchronizeOffers_WhenMultipleBoundTarget_ShouldSyncBottomAndUpperBounds()
        {
            var synchronizer = new VersionsSynchronizer();
            var actual = synchronizer.GetSynchronizeOffers(new[]
                {
                    new PackageMeta("", new PackageInfo
                    {
                        Name = "A1",
                        Dependencies = new []
                        {
                            new Dependency
                            {
                                Name = "B",
                                Version = new VersionTarget(new VersionFilter(3, null, null), new VersionFilter(4, null, null))
                            }
                        }
                    }),
                    new PackageMeta("", new PackageInfo
                    {
                        Name = "A2",
                        Dependencies = new []
                        {
                            new Dependency
                            {
                                Name = "B",
                                Version = new VersionTarget(new VersionFilter(1, null, null), new VersionFilter(2, null, null))
                            }
                        }
                    })
                })
                .ToArray();

            Assert.That(actual, Has.Length.EqualTo(1));
            Assert.That(
                actual,
                Has.Some.Matches<VersionsSynchronizer.SynchronizeVersionOffer>(
                    x => x.SourceMeta.PackageInfo.Name == "A2"
                         && x.DependencyToUpdate == "B"
                         && x.NewDependencyTarget.TargetRange.Lower == new VersionFilter(3, null, null)
                         && x.NewDependencyTarget.TargetRange.Upper == new VersionFilter(4, null, null)));
        }

        [Test]
        public void GetSynchronizeOffers_WhenMultipleBoundTargetForMultiplePackages_ShouldDoNothing()
        {
            var synchronizer = new VersionsSynchronizer();
            var actual = synchronizer.GetSynchronizeOffers(new[]
                {
                    new PackageMeta("", new PackageInfo
                    {
                        Name = "A1",
                        Dependencies = new []
                        {
                            new Dependency
                            {
                                Name = "B1",
                                Version = new VersionTarget(new VersionFilter(3, null, null), new VersionFilter(4, null, null))
                            }
                        }
                    }),
                    new PackageMeta("", new PackageInfo
                    {
                        Name = "A2",
                        Dependencies = new []
                        {
                            new Dependency
                            {
                                Name = "B2",
                                Version = new VersionTarget(new VersionFilter(1, null, null), new VersionFilter(2, null, null))
                            }
                        }
                    })
                })
                .ToArray();

            Assert.That(actual, Is.Empty);
        }
    }
}