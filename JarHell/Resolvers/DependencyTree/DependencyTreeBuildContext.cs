using System.Collections.Generic;

namespace JarHell.Resolvers.DependencyTree
{
    internal class DependencyTreeBuildContext
    {
        public HashSet<string> Resolved { get; set; }
    }
}