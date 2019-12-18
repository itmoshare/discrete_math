using System.Linq;
using JarHell.Packages;
using JetBrains.Annotations;

namespace JarHell.Optimizers
{
    public class StructureValidator
    {
        public ValidationError[] ValidateStructure(PackageMeta[] localPackages)
        {
            var multipleLocalVersionsErrors = localPackages
                .GroupBy(x => (x.PackageInfo.Name, x.PackageInfo.Version.Major))
                .Where(x => x.Count() > 1)
                .Select(x => new ValidationError("Multiple versions of one local package were found " +
                                                  $"{x.First().PackageInfo.Name} {string.Join(' ', x.Select(y => $"({y.PackageInfo.Version})"))}. " +
                                                  "Please, remove one of them."))
                .ToArray();

            return multipleLocalVersionsErrors;
        }

        public class ValidationError
        {
            [CanBeNull]
            public string Description { get; }

            public ValidationError(string description)
            {
                Description = description;
            }
        }
    }
}