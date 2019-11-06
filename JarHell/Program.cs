using System.Collections.Generic;
using CommandLine;

namespace JarHell
{
    internal class Program
    {
        class Options
        {
            [Option('p', "path", Required = true, HelpText = "Path to packages")]
            public IEnumerable<string> Paths { get; set; }
        }

        private static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(o =>
                {

                });
        }
    }
}