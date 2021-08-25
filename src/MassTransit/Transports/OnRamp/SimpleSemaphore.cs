using MassTransit.Context;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class SimpleSemaphore : IOnRampSemaphore
    {
        private readonly object syncRoot = new object();
        private readonly Dictionary<Guid, HashSet<string>> threadLocks = new Dictionary<Guid, HashSet<string>>();

        private readonly HashSet<string> locks = new HashSet<string>();

        /// <summary>
        /// Grants a lock on the identified resource to the calling thread (blocking
        /// until it is available).
        /// </summary>
        /// <returns>True if the lock was obtained.</returns>
        public virtual Task<bool> ObtainLock(
            Guid requestorId,
            ILockRepository? repository,
            string lockName,
            CancellationToken cancellationToken = default)
        {
            LogContext.Debug?.Log($"Lock '{lockName}' is desired by: {requestorId}");

            lock (syncRoot)
            {
                if (!IsLockOwner(requestorId, lockName))
                {
                    LogContext.Debug?.Log($"Lock '{lockName}' is being obtained: {requestorId}");

                    while (locks.Contains(lockName))
                    {
                        try
                        {
                            Monitor.Wait(syncRoot);
                        }
                        catch (ThreadInterruptedException)
                        {
                            LogContext.Debug?.Log($"Lock '{lockName}' was not obtained by: {requestorId}");
                        }
                    }

                    if (!threadLocks.TryGetValue(requestorId, out var requestorLocks))
                    {
                        requestorLocks = new HashSet<string>();
                        threadLocks[requestorId] = requestorLocks;
                    }
                    requestorLocks.Add(lockName);
                    locks.Add(lockName);

                    LogContext.Debug?.Log($"Lock '{lockName}' given to: {requestorId}");
                }
                else if (LogContext.Debug != null)
                {
                    LogContext.Debug?.Log($"Lock '{lockName}' already owned by: {requestorId} -- but not owner!", new Exception("stack-trace of wrongful returner"));
                }

                return Task.FromResult(true);
            }
        }

        /// <summary> Release the lock on the identified resource if it is held by the calling
        /// thread.
        /// </summary>
        public virtual Task ReleaseLock(
            Guid requestorId,
            string lockName,
            CancellationToken cancellationToken = default)
        {
            lock (syncRoot)
            {
                if (IsLockOwner(requestorId, lockName))
                {
                    if (threadLocks.TryGetValue(requestorId, out var requestorLocks))
                    {
                        requestorLocks.Remove(lockName);
                        if (requestorLocks.Count == 0)
                        {
                            threadLocks.Remove(requestorId);
                        }
                    }
                    locks.Remove(lockName);

                    LogContext.Debug?.Log($"Lock '{lockName}' returned by: {requestorId}");

                    Monitor.PulseAll(syncRoot);
                }
                else if (LogContext.Warning != null)
                {
                    LogContext.Warning?.Log($"Lock '{lockName}' attempt to return by: {requestorId} -- but not owner!", new Exception("stack-trace of wrongful returner"));
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Determine whether the calling thread owns a lock on the identified
        /// resource.
        /// </summary>
        private bool IsLockOwner(Guid requestorId, string lockName)
        {
            return threadLocks.TryGetValue(requestorId, out var requestorLocks) && requestorLocks.Contains(lockName);
        }

        /// <summary>
        /// Whether this Semaphore implementation requires a database connection for
        /// its lock management operations.
        /// </summary>
        /// <value></value>
        /// <seealso cref="IsLockOwner"/>
        /// <seealso cref="ObtainLock"/>
        /// <seealso cref="ReleaseLock"/>
        public bool RequiresConnection => false;
    }
}
