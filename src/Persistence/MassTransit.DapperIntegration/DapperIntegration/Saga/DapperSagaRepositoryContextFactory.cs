namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Saga;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Options;
    using SqlBuilders;


    public class DapperSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>,
        IQuerySagaRepositoryContextFactory<TSaga>,
        ILoadSagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;
        readonly IOptions<DapperOptions<TSaga>> _options;
        readonly IServiceProvider _serviceProvider;

        public DapperSagaRepositoryContextFactory(
            IOptions<DapperOptions<TSaga>> options, 
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            IServiceProvider serviceProvider)
        {
            _options = options;
            _factory = factory;
            _serviceProvider = serviceProvider;
        }

        public Task<T> Execute<T>(Func<LoadSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteAsyncMethod(asyncMethod, cancellationToken);
        }

        public Task<T> Execute<T>(Func<QuerySagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteAsyncMethod(asyncMethod, cancellationToken);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "dapper");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            await using var databaseContext = await CreateDatabaseContext(context.CancellationToken).ConfigureAwait(false);

            var repositoryContext = new DapperSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

            await next.Send(repositoryContext).ConfigureAwait(false);
            await databaseContext.CommitAsync().ConfigureAwait(false);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            await using var databaseContext = await CreateDatabaseContext(context.CancellationToken).ConfigureAwait(false);

            var instances = await databaseContext.QueryAsync(query.FilterExpression, context.CancellationToken).ConfigureAwait(false);

            var repositoryContext = new DapperSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);
            var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

            await next.Send(queryContext).ConfigureAwait(false);
            await databaseContext.CommitAsync().ConfigureAwait(false);
        }

        internal async Task<T> ExecuteAsyncMethod<T>(Func<DapperSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            await using var databaseContext = await CreateDatabaseContext(cancellationToken).ConfigureAwait(false);
            var sagaRepositoryContext = new DapperSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

            var result = await asyncMethod(sagaRepositoryContext).ConfigureAwait(false);
            await databaseContext.CommitAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }

        internal async Task<DatabaseContext<TSaga>> CreateDatabaseContext(CancellationToken cancellationToken)
        {
            var options = _options.Value;
            var factory = CreateContextFactory(options);

            var connection = options.DbConnectionProvider?.Invoke(_serviceProvider)
                ?? new SqlConnection(options.ConnectionString);
            
            DbTransaction transaction = null;
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                }

                // serializable was the default prior to refactor
                var isolationLevel = options.IsolationLevel.GetValueOrDefault(IsolationLevel.Serializable);

            #if NETSTANDARD2_0 || NET472
                transaction = connection.BeginTransaction(isolationLevel);
            #else
                transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken);
            #endif
                
                var contextFactory = factory(_serviceProvider);
                return contextFactory(connection, transaction);
            }
            catch (Exception)
            {
            #if NETSTANDARD2_0 || NET472
                transaction?.Dispose();
                connection.Dispose();
            #else
                if (transaction is not null)
                    await transaction.DisposeAsync();

                await connection.DisposeAsync();
            #endif
                throw;
            }
        }

        Func<IServiceProvider, DatabaseContextFactory<TSaga>> CreateContextFactory(DapperOptions<TSaga> options)
        {
            var factory = options.ContextFactoryProvider;

            if (factory is not null)
                return factory;

            var sqlBuilder = options.SqlBuilderProvider;
            var tableName = options.TableName;
            var idColumnName = options.IdColumnName;

            if (options.Provider == DatabaseProviders.Unspecified)
            {
                // legacy implementation path, which does not use a Formatter at all
                return _ => (c, t) => new DapperDatabaseContext<TSaga>(c, t);
            }

            sqlBuilder ??= options.Provider switch
            {
                DatabaseProviders.Postgres => _ => new PostgresSagaFormatter<TSaga>(tableName, idColumnName),
                DatabaseProviders.SqlServer => _ => new SqlServerSagaFormatter<TSaga>(tableName, idColumnName),
                _ => throw new InvalidOperationException($"Invalid Provider, expecting Postgres or SqlServer, instead got {options.Provider:G}")
            };

            return _ => (c, t) => new SagaDatabaseContext<TSaga>(c, t, sqlBuilder(_serviceProvider));
        }
    }
}
