namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using Microsoft.Data.SqlClient;


    public class DapperDatabaseContext<TSaga> :
        DatabaseContext<TSaga> where TSaga : class, ISaga
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

        public async Task InsertAsync(TSaga instance, CancellationToken cancellationToken)
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

        public async Task UpdateAsync(TSaga instance, CancellationToken cancellationToken)
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

        public async Task DeleteAsync(TSaga instance, CancellationToken cancellationToken)
        {
            var correlationId = instance?.CorrelationId ?? throw new ArgumentNullException(nameof(instance));

            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _connection.QueryAsync($"DELETE FROM {GetTableName()} WHERE CorrelationId = @correlationId", new { correlationId }, _transaction)
                    .ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task<TSaga> LoadAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await _connection.QuerySingleOrDefaultAsync<TSaga>(
                    $"SELECT * FROM {GetTableName()} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @correlationId",
                    new { correlationId }, _transaction).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task<IEnumerable<TSaga>> QueryAsync(Expression<Func<TSaga, bool>> filterExpression, CancellationToken cancellationToken)
        {
            var tableName = GetTableName();

            var (whereStatement, parameters) = WhereStatementHelper.GetWhereStatementAndParametersFromExpression(filterExpression);

            await _inUse.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await _connection.QueryAsync<TSaga>($"SELECT * FROM {tableName} WITH (UPDLOCK, ROWLOCK) {whereStatement}", parameters, _transaction)
                    .ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public ValueTask DisposeAsync()
        {
            _inUse.Dispose();
            _transaction.Dispose();
            _connection.Dispose();

            return default;
        }

        static string GetTableName()
        {
            return typeof(TSaga).GetCustomAttribute<TableAttribute>()?.Name ?? $"{typeof(TSaga).Name}s";
        }
    }
}
