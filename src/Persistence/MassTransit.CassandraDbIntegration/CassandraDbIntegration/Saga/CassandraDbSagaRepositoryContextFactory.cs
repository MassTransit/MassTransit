namespace MassTransit.CassandraDbIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cassandra;
    using MassTransit.Saga;


    public class CassandraDbSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>,
        ILoadSagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly Func<ISession> _databaseFactory;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;
        readonly CassandraDbSagaRepositoryOptions<TSaga> _options;

        public CassandraDbSagaRepositoryContextFactory(ISession dbContext, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            CassandraDbSagaRepositoryOptions<TSaga> options)
        {
            _databaseFactory = () => dbContext;

            _factory = factory;
            _options = options;
        }

        public CassandraDbSagaRepositoryContextFactory(Func<ISession> databaseFactory, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            CassandraDbSagaRepositoryOptions<TSaga> options)
        {
            _databaseFactory = databaseFactory;

            _factory = factory;
            _options = options;
        }

        public async Task<T> Execute<T>(Func<LoadSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new CassandraDbDatabaseContext<TSaga>(database, _options);
            var repositoryContext = new CassandraDbSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

            return await asyncMethod(repositoryContext).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "Cassandradb");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new CassandraDbDatabaseContext<TSaga>(database, _options);
            var repositoryContext = new CassandraDbSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

            await next.Send(repositoryContext).ConfigureAwait(false);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            throw new NotImplementedByDesignException("SendQuery was not implemented for cassandra.");
        }
    }
}
