using MassTransit.Context;
using MassTransit.Transports.Outbox.Configuration;
using MassTransit.Transports.Outbox.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class ClusterManager : IClusterManager
    {
        private readonly OutboxInstanceState _state;
        private readonly IClusterRepository _repository;
        private readonly ILogger _logger;
        private readonly IOutboxTransportOptions _outboxTransportOptions;
        private readonly IOutboxSemaphore _semaphore; // singleton to combo handling thread and/or db locking

        public ClusterManager(
            OutboxInstanceState state,
            IClusterRepository repository,
            ILogger<ClusterManager> logger,
            IOutboxSemaphore semaphore,
            IOutboxTransportOptions outboxOptions)
        {
            _state = state;
            _repository = repository;
            _logger = logger;
            _semaphore = semaphore;
            _outboxTransportOptions = outboxOptions;
        }

        public async Task CheckIn(Guid requestorId, CancellationToken cancellationToken)
        {
            await _repository.BeginTransactionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            var instanceLockOwner = false;
            var messageLockOwner = false;

            try
            {
                IReadOnlyList<OutboxSweeper> failedSweepers = null;
                if (!_state.FirstCheckin)
                {
                    failedSweepers = await ClusterCheckIn(cancellationToken).ConfigureAwait(false);
                    await _repository.CommitTransactionAsync(true, cancellationToken).ConfigureAwait(false);
                }

                if (_state.FirstCheckin || failedSweepers?.Any() == true)
                {
                    await _semaphore.ObtainLock(requestorId, _repository, "INSTANCES").ConfigureAwait(false);
                    instanceLockOwner = true;

                    // Now that we own the lock, make sure we still have work to do.
                    // The first time through, we also need to make sure we update/create our sweeper record
                    if (_state.FirstCheckin)
                    {
                        failedSweepers = await ClusterCheckIn(cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        failedSweepers = await FindFailedInstances(cancellationToken).ConfigureAwait(false);
                    }

                    if (failedSweepers?.Any() == true)
                    {
                        await _semaphore.ObtainLock(requestorId, _repository, "MESSAGES").ConfigureAwait(false);
                        messageLockOwner = true;

                        await ClusterRecover(failedSweepers, cancellationToken).ConfigureAwait(false);
                    }
                }

                await _repository.CommitTransactionAsync(false, cancellationToken).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                try
                {
                    await _repository.RollbackTransactionAsync(SqlExceptionUtils.IsTransient(e), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ee)
                {
                    // Log failed to rollback and swallow
                    _logger.LogError(ee, "Failed to rollback transaction");
                }
                throw;
            }
            finally
            {
                try
                {
                    await ReleaseLock(requestorId, "MESSAGES", messageLockOwner, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    await ReleaseLock(requestorId, "INSTANCES", instanceLockOwner, cancellationToken).ConfigureAwait(false);
                }
            }

            _state.FirstCheckin = false;
        }

        private async Task ReleaseLock(
            Guid requestorId,
            string lockName,
            bool doIt,
            CancellationToken cancellationToken)
        {
            if (doIt)
            {
                try
                {
                    await _semaphore.ReleaseLock(requestorId, lockName, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogContext.Error?.Log("Error releasing lock: " + e.Message, e);
                }
            }
        }

        private async Task ClusterRecover(IReadOnlyList<OutboxSweeper> failedSweepers, CancellationToken cancellationToken)
        {
            // We loop through the failed sweepers, and for each one we look in the Messages Table for ones assigned
            // to the SweeperId. We will remove the InstanceId from them, so they can be tried by another sweeper

            if (failedSweepers?.Any() == true)
            {
                try
                {
                    foreach (var sweeper in failedSweepers)
                    {
                        await _repository.FreeMessagesFromFailedSweeperInstance(_outboxTransportOptions.OutboxName, sweeper.InstanceId, cancellationToken).ConfigureAwait(false);

                        // then the last thing is to delete the failedSweeper from the table (if it is NOT _state.SweeperId, and if it does exist in the Sweepers table.. it might not if it was an orphaned one)
                        if (sweeper.InstanceId.Equals(_state.InstanceId) == false)
                        {
                            // also remove the sweeper, because it's no longer active
                            await _repository.RemoveSweeper(_outboxTransportOptions.OutboxName, sweeper.InstanceId, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    throw new OutboxException("Failure recovering messages in cluster: " + e.Message, e);
                }

            }
        }

        private async Task<IReadOnlyList<OutboxSweeper>> ClusterCheckIn(CancellationToken cancellationToken)
        {
            var failedInstances = await FindFailedInstances(cancellationToken).ConfigureAwait(false);
            try
            {
                // check in...
                _state.LastCheckin = DateTime.UtcNow;

                if (!await _repository.UpdateSweeper(_outboxTransportOptions.OutboxName, _state.InstanceId, _state.LastCheckin, cancellationToken).ConfigureAwait(false))
                {
                    await _repository.InsertSweeper(_outboxTransportOptions.OutboxName, _state.InstanceId, _state.LastCheckin, _outboxTransportOptions.ClusterCheckinInterval, cancellationToken).ConfigureAwait(false);
                }

                await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new OutboxException("Failure updating sweeper state when checking-in: " + e.Message, e);
            }

            return failedInstances;
        }

        private async Task<IReadOnlyList<OutboxSweeper>> FindFailedInstances(CancellationToken cancellationToken)
        {
            try
            {
                var failedInstances = new List<OutboxSweeper>();

                var foundThisSweeper = false;

                var sweepers = await _repository.GetAllSweepers(_outboxTransportOptions.OutboxName, cancellationToken).ConfigureAwait(false);

                foreach (var sweeper in sweepers)
                {
                    // find own record...
                    if (sweeper.InstanceId == _state.InstanceId)
                    {
                        foundThisSweeper = true;
                        if (_state.FirstCheckin)
                        {
                            failedInstances.Add(sweeper);
                        }
                    }
                    else
                    {
                        // find failed instances...
                        if (CalcFailedIfAfter(sweeper) < DateTime.UtcNow)
                        {
                            failedInstances.Add(sweeper);
                        }
                    }
                }

                // The first time through, also check for failed orphaned messages.
                if (_state.FirstCheckin)
                {
                    // TODO, query the Messages to see if there are any that are assigned an sweeperId, which doesn't exist in the Sweepers table.
                    //failedInstances.AddRange(await FindOrphanedFailedInstances(conn, states, cancellationToken).ConfigureAwait(false));
                    var orphanedSweepers = await FindOrphanedFailedInstances(sweepers, cancellationToken).ConfigureAwait(false);
                    failedInstances.AddRange(orphanedSweepers);
                }

                // If not the first time but we didn't find our own instance, then
                // Someone must have done recovery for us.
                if (!foundThisSweeper && !_state.FirstCheckin)
                {
                    // TODO: revisit when handle self-failed-out impl'ed (see TODO in clusterCheckIn() below)
                    LogContext.Warning?.Log(
                        "This scheduler instance (" + _state.InstanceId + ") is still " +
                        "active but was recovered by another instance in the cluster.  " +
                        "This may cause inconsistent behavior.");
                }

                return failedInstances;

            }
            catch (Exception e)
            {
                _state.LastCheckin = DateTime.UtcNow;
                throw new OutboxException("Failure identifying failed instances when checking-in: "
                                                  + e.Message, e);
            }


        }

        private async Task<IReadOnlyList<OutboxSweeper>> FindOrphanedFailedInstances(
            IReadOnlyCollection<OutboxSweeper> sweepers,
            CancellationToken cancellationToken)
        {
            var orphanedInstances = new List<OutboxSweeper>();

            var instanceIds = await _repository.GetMessageSweeperInstanceIds(_outboxTransportOptions.OutboxName, cancellationToken).ConfigureAwait(false);

            var allMessageSweeperInstanceIds = new HashSet<string>(instanceIds);
            if (allMessageSweeperInstanceIds.Any())
            {
                foreach (var sweeper in sweepers)
                {
                    allMessageSweeperInstanceIds.Remove(sweeper.InstanceId);
                }

                foreach (string instanceId in allMessageSweeperInstanceIds)
                {
                    var orphanedInstance = new OutboxSweeper();
                    orphanedInstance.InstanceId = instanceId;

                    orphanedInstances.Add(orphanedInstance);

                    LogContext.Warning?.Log("Found orphaned fired triggers for instance: " + orphanedInstance.InstanceId);
                }
            }

            return orphanedInstances;
        }

        protected DateTimeOffset CalcFailedIfAfter(OutboxSweeper sweeper)
        {
            TimeSpan passed = DateTime.UtcNow - _state.LastCheckin;
            TimeSpan ts = sweeper.CheckinInterval > passed ? sweeper.CheckinInterval : passed;
            return sweeper.LastCheckinTime.Add(ts).Add(_outboxTransportOptions.ClusterCheckInMisfireInterval);
        }

        public async Task RecoverMessagesAndCleanup(Guid requestorId, CancellationToken cancellationToken)
        {
            await _repository.BeginTransactionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            try
            {
                await _semaphore.ObtainLock(requestorId, _repository, "MESSAGES", cancellationToken).ConfigureAwait(false);
                await _repository.FreeAllMessagesFromAnySweeperInstance(_outboxTransportOptions.OutboxName, cancellationToken).ConfigureAwait(false);
                await _repository.CommitTransactionAsync(false, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                try
                {
                    await _repository.RollbackTransactionAsync(SqlExceptionUtils.IsTransient(e), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // Log failed to rollback and swallow
                    _logger.LogError(e, "Failed to rollback transaction");
                }
                throw;
            }
            finally
            {
                await _semaphore.ReleaseLock(requestorId, "MESSAGES", cancellationToken).ConfigureAwait(false);
                _state.FirstCheckin = false; // Although this isn't really a check in, it signals to the sweeper hosted service that it can now start sweeping
            }
        }
    }
}
