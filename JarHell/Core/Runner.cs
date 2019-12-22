using System;
using System.Linq;
using System.Threading;

namespace JarHell.Core
{
    public class Runner
    {
        public void Run(ResolvedPackage root)
        {
            var runWaitersRepository = new RunWaitersRepository();
            RegisterRunWaiters(runWaitersRepository, root);
            runWaitersRepository.RunAllNonLocked().GetAwaiter().GetResult();
        }

        private void RegisterRunWaiters(RunWaitersRepository runWaitersRepository, ResolvedPackage package)
        {
            void RunAction(ResolvedPackage t)
            {
                Thread.Sleep(100);
                Console.WriteLine($"{t.PackageMeta.PackageInfo.Name} {t.PackageMeta.PackageInfo.Version} was started");
                runWaitersRepository.NotifyRunAsync(t).GetAwaiter().GetResult();
            }

            if (!package.ResolvedDependencies.Any())
            {
                runWaitersRepository.AddEmptyRunWaiter(package, RunAction);
            }

            foreach (var dependency in package.ResolvedDependencies)
            {
                runWaitersRepository.AddRunWaiter(
                    package,
                    dependency,
                    RunAction);
                RegisterRunWaiters(runWaitersRepository, dependency);
            }
        }
    }
}