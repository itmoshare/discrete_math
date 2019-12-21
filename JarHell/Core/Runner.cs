using System;
using System.Collections.Generic;
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
            foreach (var dependency in package.ResolvedDependencies)
            {
                runWaitersRepository.AddRunWaiter(
                    package,
                    dependency,
                    t =>
                    {
                        Thread.Sleep(100);
                        Console.WriteLine($"{t.PackageMeta.PackageInfo.Name} {t.PackageMeta.PackageInfo.Version} was started");
                        runWaitersRepository.NotifyRunAsync(t).GetAwaiter().GetResult();
                    });
                RegisterRunWaiters(runWaitersRepository, dependency);
            }
        }
    }
}