namespace MassTransit.RedisIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using StackExchange.Redis;


    public class RedisSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly Func<IDatabase> _databaseFactory;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;
        readonly RedisSagaRepositoryOptions<TSaga> _options;

        public RedisSagaRepositoryContextFactory(ConnectionMultiplexer multiplexer, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            RedisSagaRepositoryOptions<TSaga> options)
        {
            IDatabase DatabaseFactory()
            {
                return options.DatabaseSelector(multiplexer);
            }

            _databaseFactory = DatabaseFactory;

            _factory = factory;
            _options = options;
        }

        public RedisSagaRepositoryContextFactory(Func<IDatabase> databaseFactory, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            RedisSagaRepositoryOptions<TSaga> options)
        {
            _databaseFactory = databaseFactory;

            _factory = factory;
            _options = options;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "redis");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new RedisDatabaseContext<TSaga>(database, _options);
            try
            {
                if (_options.ConcurrencyMode == ConcurrencyMode.Pessimistic)
                    await databaseContext.Lock(context.CorrelationId.Value, context.CancellationToken).ConfigureAwait(false);

                var repositoryContext = new RedisSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

                await next.Send(repositoryContext).ConfigureAwait(false);
            }
            finally
            {
                await databaseContext.DisposeAsync().ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            throw new NotImplementedByDesignException("Redis saga repository does not support queries");
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new RedisDatabaseContext<TSaga>(database, _options);
            try
            {
                var repositoryContext = new RedisSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

                return await asyncMethod(repositoryContext).ConfigureAwait(false);
            }
            finally
            {
                await databaseContext.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
