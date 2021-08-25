using MassTransit.Context;
using MassTransit.Serialization;
using MassTransit.Transports.Outbox;
using MassTransit.Transports.Outbox.Configuration;
using MassTransit.Transports.Outbox.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.EntityFrameworkCoreIntegration.Outbox
{
    public class DbContextOutboxRepository<TDbContext>
        : IClusterRepository
        , IOnRampTransportRepository
        , ISweeperRepository
        , ILockRepository
        where TDbContext : DbContext
    {
        private readonly TDbContext _db;
        private readonly IOnRampTransportOptions _outboxTransportOptions;
        private readonly IRepositoryStatementProvider _statementProvider;
        private IDbContextTransaction _currentTransaction;

        public DbContextOutboxRepository(TDbContext db, IRepositoryStatementProvider statementProvider, IOnRampTransportOptions outboxTransportOptions)
        {
            _db = db;
            _outboxTransportOptions = outboxTransportOptions;
            _statementProvider = statementProvider;
        }

        #region IConnectionAndTransactionHolder
        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }
            _currentTransaction = await _db.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync(bool createAnother = false, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null) return;

            // Have to get the isolation level before commit, otherwise ef throws an error when trying to get the underlying transaction
            var currentIsolation = _currentTransaction.GetDbTransaction().IsolationLevel;

            try
            {
                await _currentTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
                if (createAnother) _currentTransaction = await _db.Database.BeginTransactionAsync(currentIsolation, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task RollbackTransactionAsync(bool transientError, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null) return;

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (transientError)
                {
                    // original error was transient, ones we have in Azure, don't complain too much about it
                    // we will try again anyway
                    LogContext.Debug?.Log("Rollback failed due to transient error");
                }
                else
                {
                    LogContext.Error?.Log("Couldn't rollback ADO.NET connection. " + e.Message, e);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion IConnectionAndTransactionHolder

        #region IClusterRepository
        public virtual async Task FreeMessagesFromFailedSweeperInstance(string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            await _db.Database.ExecuteSqlRawAsync(_statementProvider.FreeMessagesFromFailedSweeperInstanceStatement(), new object[] { outboxName, instanceId }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<OnRampSweeper>> GetAllSweepers(string outboxName, CancellationToken cancellationToken = default)
        {
            return await _db.Set<OnRampSweeper>().AsNoTracking().Where(x => x.OnRampName == outboxName).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetMessageSweeperInstanceIds(string outboxName, CancellationToken cancellationToken = default)
        {
            return await _db.Set<OnRampMessage>().Where(x => x.OnRampName == outboxName && x.InstanceId != null).Select(x => x.InstanceId).Distinct().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task InsertSweeper(string outboxName, string instanceId, DateTime lastCheckin, TimeSpan checkinInterval, CancellationToken cancellationToken = default)
        {
            _db.Set<OnRampSweeper>().Add(new OnRampSweeper { OnRampName = outboxName, InstanceId = instanceId, LastCheckinTime = lastCheckin, CheckinInterval = checkinInterval });

            return Task.CompletedTask;
        }

        public async Task RemoveSweeper(string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            var sweeper = await _db.Set<OnRampSweeper>().FindAsync(new object[] { outboxName, instanceId }, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (sweeper != null) _db.Set<OnRampSweeper>().Remove(sweeper);
        }

        public async Task<bool> UpdateSweeper(string outboxName, string instanceId, DateTime lastCheckin, CancellationToken cancellationToken = default)
        {
            var sweeper = await _db.Set<OnRampSweeper>().FindAsync(new object[] { outboxName, instanceId }, cancellationToken).ConfigureAwait(false);

            if (sweeper == null) return false;

            sweeper.LastCheckinTime = lastCheckin;

            return true;
        }

        public virtual async Task FreeAllMessagesFromAnySweeperInstance(string outboxName, CancellationToken cancellationToken = default)
        {
            await _db.Database.ExecuteSqlRawAsync(_statementProvider.FreeAllMessagesFromAnySweeperInstanceStatement(), new object[] { outboxName }, cancellationToken).ConfigureAwait(false);
        }
        #endregion IClusterRepository

        #region ISweeperRepository
        public async Task FailedToSendMessage(OnRampSweeperSendException exception, CancellationToken cancellationToken = default)
        {
            await _db.Database.ExecuteSqlRawAsync(_statementProvider.FailedToSendMessageStatement(), new object[] { exception.OnRampMessage.OnRampName, exception.OnRampMessage.Id }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<OnRampMessage>> FetchNextMessages(string outboxName, int prefetchCount, CancellationToken cancellationToken = default)
        {
            return await _db.Set<OnRampMessage>().AsNoTracking().Where(x => x.OnRampName == outboxName && x.InstanceId == null).OrderBy(x => x.Added).Take(prefetchCount).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual Task RemoveMessage(OnRampMessage message, CancellationToken cancellationToken = default)
        {
            return _db.Database.ExecuteSqlRawAsync(_statementProvider.RemoveMessageStatement(), new object[] { message.OnRampName, message.Id }, cancellationToken);
        }

        public virtual async Task ReserveMessages(IEnumerable<Guid> enumerable, string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            var ids = enumerable.Select(x => x.ToString().ToUpper()).ToList();
            var placeholders = string.Join(",", Enumerable.Range(0, ids.Count).Select(i => "{" + (i + 2) + "}"));
            var query = _statementProvider.ReserveMessagesStatement();
            query = query.Replace("{2}", $"{placeholders}");
            ids.Insert(0, outboxName);
            ids.Insert(0, instanceId);
            var values = ids.Cast<object>().ToArray();
            await _db.Database.ExecuteSqlRawAsync(query, values, cancellationToken).ConfigureAwait(false);
        }

        public Task RemoveAllMessages(string outboxName, string instanceId, CancellationToken cancellationToken = default)
        {
            return _db.Database.ExecuteSqlRawAsync(_statementProvider.RemoveAllMessagesStatement(), new object[] { outboxName, instanceId }, cancellationToken);
        }

        public async Task RemoveAllCompletedMessages(List<OnRampMessage> completedMessages, CancellationToken cancellationToken = default)
        {
            if (!completedMessages.Any()) return;
            var ids = completedMessages.Select(x => x.ToString().ToUpper()).ToList();
            var placeholders = string.Join(",", Enumerable.Range(0, ids.Count).Select(i => "{" + (i + 1) + "}"));
            var query = _statementProvider.RemoveAllCompletedMessagesStatement();
            query = query.Replace("{1}", $"{placeholders}");
            ids.Insert(0, completedMessages.First().OnRampName);
            var values = ids.Cast<object>().ToArray();
            await _db.Database.ExecuteSqlRawAsync(query, values, cancellationToken).ConfigureAwait(false);
        }

        public Task FailedToSendMessages(List<OnRampMessage> failedMessages, string instanceId, CancellationToken cancellationToken = default)
        {
            if (!failedMessages.Any()) return Task.CompletedTask;
            return _db.Database.ExecuteSqlRawAsync(_statementProvider.FailedToSendMessagesStatement(), new object[] { failedMessages[0].OnRampName, instanceId }, cancellationToken);
        }
        #endregion ISweeperRepository

        #region IOutboxTransportRepository
        public Task InsertMessage(JsonSerializedMessage message, CancellationToken cancellationToken = default) // this is called by the adding method, so we don't call save changes async. let that be up to the calling app
        {
            try
            {
                // To speed up inserts, turn off changetracker
                _db.ChangeTracker.AutoDetectChangesEnabled = false;
                _db.Set<OnRampMessage>().Add(new OnRampMessage { OnRampName = _outboxTransportOptions.OnRampName, Id = NewId.NextGuid(), SerializedMessage = message, Added = DateTime.UtcNow });
            }
            finally
            {
                _db.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return Task.CompletedTask;
        }
        #endregion IOutboxTransportRepository

        #region ILockRepository
        public virtual async Task ObtainLock(string outboxName, string lockName)
        {
            var @lock = await _db.Set<OnRampLock>().FromSqlRaw(_statementProvider.SelectRowLockStatement(), outboxName, lockName).FirstOrDefaultAsync();

            if (@lock == null)
            {
                _db.Set<OnRampLock>().Add(new OnRampLock { OnRampName = outboxName, LockName = lockName });
                await _db.SaveChangesAsync();
            }
        }
        #endregion ILockRepository
    }
}
