namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using Microsoft.Data.SqlClient;


    public class DapperDatabaseContext<TSaga> :
        DatabaseContext<TSaga>,
        IDisposable
    {
        readonly SqlConnection _connection;
        readonly SemaphoreSlim _inUse;
        readonly SqlTransaction _transaction;

        public DapperDatabaseContext(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
            _inUse = new SemaphoreSlim(1);
        }

        public async Task InsertAsync<T>(T instance, CancellationToken cancellationToken)
            where T : class, ISaga
        {
            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _connection.InsertAsync(instance, _transaction).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task UpdateAsync<T>(T instance, CancellationToken cancellationToken)
            where T : class, ISaga
        {
            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _connection.UpdateAsync(instance, _transaction).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task DeleteAsync<T>(T instance, CancellationToken cancellationToken)
            where T : class, ISaga
        {
            var correlationId = instance?.CorrelationId ?? throw new ArgumentNullException(nameof(instance));

            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _connection.QueryAsync($"DELETE FROM {GetTableName<T>()} WHERE CorrelationId = @correlationId", new {correlationId}, _transaction)
                    .ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task<T> LoadAsync<T>(Guid correlationId, CancellationToken cancellationToken)
            where T : class, ISaga
        {
            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await _connection.QuerySingleOrDefaultAsync<T>(
                    $"SELECT * FROM {GetTableName<T>()} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @correlationId",
                    new {correlationId}, _transaction).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(Expression<Func<T, bool>> filterExpression, CancellationToken cancellationToken)
            where T : class, ISaga
        {
            var tableName = GetTableName<T>();

            var (whereStatement, parameters) = WhereStatementHelper.GetWhereStatementAndParametersFromExpression(filterExpression);

            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await _connection.QueryAsync<T>($"SELECT * FROM {tableName} WITH (UPDLOCK, ROWLOCK) {whereStatement}", parameters, _transaction)
                    .ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public void Dispose()
        {
            _inUse.Dispose();
            _transaction.Dispose();
            _connection.Dispose();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        static string GetTableName<T>()
        {
            return $"{typeof(T).Name}s";
        }
    }
}
