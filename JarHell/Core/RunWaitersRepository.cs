using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JarHell.Core
{
    public class RunWaitersRepository
    {
        private ConcurrentDictionary<ResolvedPackage, List<ResolvedPackage>> _waiters = new ConcurrentDictionary<ResolvedPackage, List<ResolvedPackage>>();

        private ConcurrentDictionary<ResolvedPackage, Dictionary<ResolvedPackage, bool>> _waitsTo = new ConcurrentDictionary<ResolvedPackage, Dictionary<ResolvedPackage, bool>>();

        private ConcurrentDictionary<ResolvedPackage, Action<ResolvedPackage>> _waitCompletedCallbacks = new ConcurrentDictionary<ResolvedPackage, Action<ResolvedPackage>>();

        public void AddRunWaiter(
            ResolvedPackage package,
            ResolvedPackage waitsToPackage,
            Action<ResolvedPackage> onWaitCompleted)
        {
            _waitCompletedCallbacks[package] = onWaitCompleted;
            if (_waitsTo.TryGetValue(package, out var waitsTo))
            {
                waitsTo.Add(waitsToPackage, false);
            }
            else
            {
                _waitsTo[package] = new Dictionary<ResolvedPackage, bool>{ { waitsToPackage, false } };
            }
        }

        public async Task NotifyRunAsync(ResolvedPackage package)
        {
            if (_waiters.TryGetValue(package, out var waiters))
            {
                foreach (var waiter in waiters.Where(x => _waitsTo.ContainsKey(x) && _waitsTo[x].ContainsKey(package)))
                {
                    _waitsTo[waiter][package] = true;
                    if (_waitsTo[waiter].Values.All(x => x))
                    {
                        if (_waitCompletedCallbacks.TryGetValue(waiter, out var callback))
                        {
                            await Task.Run(() => callback(waiter));
                        }
                    }
                }
            }
        }

        public Task RunAllNonLocked()
        {
            return Task.WhenAll(_waitsTo.Where(x => !x.Value.Any()).Select(x => NotifyRunAsync(x.Key)));
        }
    }
}