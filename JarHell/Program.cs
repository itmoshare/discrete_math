using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using JarHell.Resolvers;
using JarHell.Resolvers.DependencyTree;

namespace JarHell
{
    internal class Program
    {
        class Options
        {
            [Option('t', "target", Required = true, HelpText = "Paths to target packages")]
            public IEnumerable<string> Targets { get; set; }

            [Option('p', "path", Required = true, HelpText = "Paths to find packages")]
            public IEnumerable<string> Paths { get; set; }
        }

        private static void Main(string[] args)
        {
            var packagesRepository = new PackageRepository();

            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(o =>
                {

                    foreach (var package in FilesScanner.GetAllPackages(o.Paths.ToArray(), true))
                    {
                        packagesRepository.AddPackage(package);
                    }

                    var targetPackages = FilesScanner.GetPackages(o.Targets.ToArray());
                    var dependencyTree = DependencyTreeBuilder.Build(packagesRepository, targetPackages);

                    foreach (var dep in dependencyTree.EnumerateDfs().Where(x => x.PackageName != null))
                    {
                        Console.WriteLine(dep);
                    }
                });
        }
    }
}