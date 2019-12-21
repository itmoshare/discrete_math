using System;
using System.IO;
using System.Linq;
using CommandLine;
using JarHell.Core;
using JarHell.Optimizers;
using JarHell.Optimizers.StructureOptimizer;
using JarHell.Packages;
using Newtonsoft.Json;

namespace JarHell
{
    internal class Program
    {
        class Options
        {
            [Option('t', "target", Required = true, HelpText = "Name of target package")]
            public string Target { get; set; }

            [Option('p', "path", Required = true, HelpText = "Path to find local packages")]
            public string Path { get; set; }

            [Option('r', "reppath", Required = true, HelpText = "Path to find repository packages")]
            public string RepositoryPath { get; set; }
        }

        private static void Main(string[] args)
        {
            var packagesRepository = new PackageRepository();

            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    foreach (var package in FilesScanner.GetAllPackages(new [] { o.RepositoryPath }, false))
                    {
                        packagesRepository.AddPackage(package);
                    }

                    var localPackages = FilesScanner.GetAllPackages(new [] { o.Path }, true);

                    var structureValidator = new StructureValidator();

                    var validationErrors = structureValidator.ValidateStructure(localPackages);
                    foreach (var validationError in structureValidator.ValidateStructure(localPackages))
                    {
                        Console.WriteLine(validationError);
                    }

                    if (validationErrors.Any())
                    {
                        return;
                    }

                    HandleVersionSynchronization(localPackages);
                    HandleCycles(localPackages, packagesRepository, o.Target);
                    var resolvedPackages = ResolvePackages(PackageRepository.Create(localPackages), packagesRepository, o.Target);
                    Run(resolvedPackages);
                });
        }

        private static void HandleVersionSynchronization(PackageMeta[] localPackages)
        {
            var versionsSynchronizer = new VersionsSynchronizer();

            var versionSynchronizationOffers = versionsSynchronizer.GetSynchronizeOffers(localPackages).ToArray();
            foreach (var synchronizationOffer in versionSynchronizationOffers)
            {
                var userAnswer = ConsoleAsk($"{synchronizationOffer.Description}. " +
                                            $"Source package: {synchronizationOffer.SourceMeta.PackageInfo.Name}. " +
                                            $"Dependency: {synchronizationOffer.DependencyToUpdate}. " +
                                            $"Offer update from: {synchronizationOffer.SourceMeta.PackageInfo.Dependencies.First(x => x.Name == synchronizationOffer.DependencyToUpdate)} " +
                                            $"Offer update to: {synchronizationOffer.NewDependencyTarget}");

                if (userAnswer)
                {
                    var newPackageInfo = new PackageInfo
                    {
                        Name = synchronizationOffer.SourceMeta.PackageInfo.Name,
                        Version = synchronizationOffer.SourceMeta.PackageInfo.Version,
                        Dependencies = synchronizationOffer
                            .SourceMeta
                            .PackageInfo
                            .Dependencies
                            .Select(x => x.Name != synchronizationOffer.DependencyToUpdate
                                ? new Dependency { Name = x.Name, Version = x.Version }
                                : new Dependency { Name = x.Name, Version = synchronizationOffer.NewDependencyTarget })
                            .ToArray()
                    };

                    File.WriteAllText(synchronizationOffer.SourceMeta.Path, JsonConvert.SerializeObject(newPackageInfo, Formatting.Indented));
                }
            }
        }

        private static void HandleCycles(PackageMeta[] localPackages, PackageRepository packageRepository, string target)
        {
            var structureOptimizer = StructureOptimizer.Create(localPackages, packageRepository, target);
            foreach (var optimizationOffer in structureOptimizer.CollectCycles())
            {
                Console.WriteLine("Cycle found");
                foreach (var offer in optimizationOffer)
                {
                    if (ConsoleAsk($"Remove {offer.SourceMeta.PackageInfo.Name} dependency {offer.DependencyToRemove}?"))
                    {
                        var newPackageInfo = new PackageInfo
                        {
                            Name = offer.SourceMeta.PackageInfo.Name,
                            Version = offer.SourceMeta.PackageInfo.Version,
                            Dependencies = offer
                                .SourceMeta
                                .PackageInfo
                                .Dependencies
                                .Select(x => x.Name != offer.DependencyToRemove
                                    ? new Dependency { Name = x.Name, Version = x.Version }
                                    : null)
                                .Where(x => x != null)
                                .ToArray()
                        };

                        File.WriteAllText(offer.SourceMeta.Path, JsonConvert.SerializeObject(newPackageInfo, Formatting.Indented));
                    }
                }
            }
        }

        private static ResolvedPackage ResolvePackages(
            PackageRepository local,
            PackageRepository repository,
            string target)
        {
            var packageResolver = new PackagesResolver();
            return packageResolver.Resolve(local, repository, target);
        }

        private static void Run(ResolvedPackage resolvedPackage)
        {
            var runner = new Runner();
            runner.Run(resolvedPackage);
        }

        private static bool ConsoleAsk(string question)
        {
            Console.WriteLine(question);
            Console.WriteLine("Y/N?");
            var answer = Console.ReadLine();
            if (answer == "y" || answer == "Y")
            {
                return true;
            }

            if (answer == "n" || answer == "N")
            {
                return false;
            }

            return ConsoleAsk(question);
        }
    }
}