using MassTransit.Context;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public abstract class ConnectionAndTransactionHolder : IConnectionAndTransactionHolder, IDisposable
    {
        protected readonly DbConnection _connection;
        protected DbTransaction? _transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionAndTransactionHolder"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ConnectionAndTransactionHolder(DbConnection connection)
        {
            this._connection = connection;
        }

        public async Task CommitTransactionAsync(bool openNewTransaction, CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                try
                {
                    CheckNotZombied();
                    IsolationLevel il = _transaction.IsolationLevel;
                    _transaction.Commit();
                    if (openNewTransaction)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                        // open new transaction to go with
                        await BeginTransactionAsync(il, cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    throw new OutboxException("Couldn't commit ADO.NET transaction. " + e.Message, e);
                }
                finally
                {
                    _transaction?.Dispose();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            try
            {
                _transaction?.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        public Task RollbackTransactionAsync(bool transientError, CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                try
                {
                    CheckNotZombied();
                    _transaction.Rollback();
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
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            return Task.CompletedTask;
        }

        private void CheckNotZombied()
        {
            if (_transaction != null && _transaction.Connection == null)
            {
                throw new InvalidOperationException("Transaction not connected, or was disconnected");
            }
        }

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            if (_connection.State == ConnectionState.Closed) await _connection.OpenAsync();

            _transaction = _connection.BeginTransaction(isolationLevel);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
