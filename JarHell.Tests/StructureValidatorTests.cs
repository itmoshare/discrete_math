using JarHell.Optimizers;
using JarHell.Packages;
using NUnit.Framework;
using Version = JarHell.Versions.Version;

namespace JarHell.Tests
{
    public class StructureValidatorTests
    {
        [Test]
        public void Validate_WhenMultipleMinorLocalVersions_ShouldReturnError()
        {
            var structureValidator = new StructureValidator();

            var v1 = new Version(1, 0, 1);
            var v2 = new Version(1, 0, 2);
            var actual = structureValidator.ValidateStructure(
                new[]
                {
                    new PackageMeta("", true, new PackageInfo { Name = "A", Version = v1 }),
                    new PackageMeta("", true, new PackageInfo { Name = "A", Version = v2 })
                });

            Assert.That(actual, Has.Length.EqualTo(1));
            Assert.That(actual, Has.Some.Matches<StructureValidator.ValidationError>(
                x => x.Description.Contains(v1.ToString())
                     && x.Description.Contains(v2.ToString())));
        }
    }
}