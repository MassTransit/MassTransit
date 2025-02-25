namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.Data.SqlClient;


    public class DapperSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>,
        IQuerySagaRepositoryContextFactory<TSaga>,
        ILoadSagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;
        readonly DapperOptions<TSaga> _options;

        public DapperSagaRepositoryContextFactory(DapperOptions<TSaga> options, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
        {
            _options = options;
            _factory = factory;
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
            await using DatabaseContext<TSaga> databaseContext = await CreateDatabaseContext(context.CancellationToken).ConfigureAwait(false);

            var repositoryContext = new DapperSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

            await next.Send(repositoryContext).ConfigureAwait(false);

            databaseContext.Commit();
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            await using DatabaseContext<TSaga> databaseContext = await CreateDatabaseContext(context.CancellationToken).ConfigureAwait(false);

            IEnumerable<TSaga> instances = await databaseContext.QueryAsync(query.FilterExpression, context.CancellationToken).ConfigureAwait(false);

            var repositoryContext = new DapperSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

            var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

            await next.Send(queryContext).ConfigureAwait(false);

            databaseContext.Commit();
        }

        async Task<T> ExecuteAsyncMethod<T>(Func<DapperSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            await using DatabaseContext<TSaga> databaseContext = await CreateDatabaseContext(cancellationToken).ConfigureAwait(false);
            var sagaRepositoryContext = new DapperSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

            var result = await asyncMethod(sagaRepositoryContext).ConfigureAwait(false);

            databaseContext.Commit();

            return result;
        }

        async Task<DatabaseContext<TSaga>> CreateDatabaseContext(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(_options.ConnectionString);
            SqlTransaction transaction = null;
            try
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                transaction = connection.BeginTransaction(_options.IsolationLevel);

                return _options.ContextFactory != null
                    ? _options.ContextFactory(connection, transaction)
                    : new DapperDatabaseContext<TSaga>(connection, transaction);
            }
            catch (Exception)
            {
                transaction?.Dispose();
                connection.Dispose();

                throw;
            }
        }
    }
}
