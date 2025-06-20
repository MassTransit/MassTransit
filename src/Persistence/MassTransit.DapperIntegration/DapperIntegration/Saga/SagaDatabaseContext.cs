namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Dapper;
    using SqlBuilders;

    /// <summary>
    /// Contains saga-specific logic as well as respecting ISagaVersion
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class SagaDatabaseContext<TSaga> : DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DbConnection _connection;
        readonly DbTransaction _transaction;
        readonly ISagaSqlFormatter<TSaga> _sqlFormatter;

        public SagaDatabaseContext(DbConnection connection, DbTransaction transaction, ISagaSqlFormatter<TSaga> sqlFormatter)
        {
            _connection = connection;
            _transaction = transaction;
            _sqlFormatter = sqlFormatter;
        }
    
        public Task<TSaga> LoadAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            var param = new DynamicParameters();
            param.Add("correlationId", correlationId);

            var sql = _sqlFormatter.BuildLoadSql();

            return _connection.QueryFirstOrDefaultAsync<TSaga>(sql, param, _transaction);
        }

        public Task<IEnumerable<TSaga>> QueryAsync(Expression<Func<TSaga, bool>> filterExpression, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            var sql = _sqlFormatter.BuildQuerySql(filterExpression, (k, v) => parameters.Add(k, v));
        
            return _connection.QueryAsync<TSaga>(sql, parameters, _transaction);
        }
    
        public async Task InsertAsync(TSaga instance, CancellationToken cancellationToken = default)
        {
            var sql = _sqlFormatter.BuildInsertSql();

            var rows = await ExecuteSql(instance, sql, cancellationToken).ConfigureAwait(false);

            if (rows == 0)
                throw new DapperConcurrencyException("Saga Insert failed", typeof(TSaga), instance.CorrelationId);
        }

        public async Task UpdateAsync(TSaga instance, CancellationToken cancellationToken = default)
        {
            var sql = _sqlFormatter.BuildUpdateSql();
        
            var rows = await ExecuteSql(instance, sql, cancellationToken).ConfigureAwait(false);

            if (rows == 0)
                throw new DapperConcurrencyException("Saga Update failed", typeof(TSaga), instance.CorrelationId);
        }

        public async Task DeleteAsync(TSaga instance, CancellationToken cancellationToken)
        {
            var sql = _sqlFormatter.BuildDeleteSql();

            var rows = await ExecuteSql(instance, sql, cancellationToken).ConfigureAwait(false);

            if (rows == 0)
                throw new DapperConcurrencyException("Saga Delete failed", typeof(TSaga), instance.CorrelationId);
        }

    #if NETFRAMEWORK || NETSTANDARD2_0
        public Task CommitAsync(CancellationToken token = default)
        {
            _transaction.Commit();
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _transaction.Dispose();
            _connection.Dispose();

            return default;
        }
    #else
        public Task CommitAsync(CancellationToken token = default)
        {
            return _transaction.CommitAsync(token);
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
            await _connection.DisposeAsync();
        }
    #endif

        async Task<int> ExecuteSql(TSaga instance, string sql, CancellationToken cancellationToken)
        {
            var param = new DynamicParameters();
            param.AddDynamicParams(instance);
            param.Add("correlationId", instance.CorrelationId);

            if (instance is ISagaVersion versioned)
            {
                versioned.Version++;
                param.Add("version", versioned.Version);
            }

            var rows = await _connection.ExecuteAsync(sql, param, _transaction).ConfigureAwait(false);
            return rows;
        }

        internal ISagaSqlFormatter<TSaga> SqlFormatter => _sqlFormatter;
    }
}
