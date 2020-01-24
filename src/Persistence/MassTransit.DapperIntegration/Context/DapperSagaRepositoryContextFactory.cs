namespace MassTransit.DapperIntegration.Context
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;


    public class DapperSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly DapperOptions<TSaga> _options;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public DapperSagaRepositoryContextFactory(DapperOptions<TSaga> options, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
        {
            _options = options;
            _factory = factory;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "dapper");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            using var connection = new SqlConnection(_options.ConnectionString);
            await connection.OpenAsync(context.CancellationToken).ConfigureAwait(false);

            using var transaction = connection.BeginTransaction(_options.IsolationLevel);

            using var databaseContext = new DapperDatabaseContext<TSaga>(connection, transaction);

            var repositoryContext = new DapperSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

            await next.Send(repositoryContext).ConfigureAwait(false);

            transaction.Commit();
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            using var connection = new SqlConnection(_options.ConnectionString);
            await connection.OpenAsync(context.CancellationToken).ConfigureAwait(false);

            using var transaction = connection.BeginTransaction(_options.IsolationLevel);

            using var databaseContext = new DapperDatabaseContext<TSaga>(connection, transaction);

            var instances = await databaseContext.QueryAsync(query.FilterExpression, context.CancellationToken).ConfigureAwait(false);

            var repositoryContext = new DapperSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

            var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances.ToList());

            await next.Send(queryContext).ConfigureAwait(false);

            transaction.Commit();
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            using var connection = new SqlConnection(_options.ConnectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            using var transaction = connection.BeginTransaction(_options.IsolationLevel);

            using var databaseContext = new DapperDatabaseContext<TSaga>(connection, transaction);

            var sagaRepositoryContext = new DapperSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

            var result = await asyncMethod(sagaRepositoryContext).ConfigureAwait(false);

            transaction.Commit();

            return result;
        }
    }
}
