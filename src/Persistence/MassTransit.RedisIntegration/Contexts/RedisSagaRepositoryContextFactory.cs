namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;
    using StackExchange.Redis;


    public class RedisSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly ConnectionMultiplexer _multiplexer;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;
        readonly RedisSagaRepositoryOptions<TSaga> _options;
        readonly Func<IDatabase> _databaseFactory;

        public RedisSagaRepositoryContextFactory(ConnectionMultiplexer multiplexer, ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory,
            RedisSagaRepositoryOptions<TSaga> options)
        {
            _multiplexer = multiplexer;
            _databaseFactory = GetDatabase;

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

        public async Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId = default)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new RedisDatabaseContext<TSaga>(database, _options);

            if (correlationId.HasValue && !_options.Optimistic)
                await databaseContext.Lock(correlationId.Value, context.CancellationToken).ConfigureAwait(false);

            return new RedisSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);
        }

        public async Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var database = _databaseFactory();

            var context = new RedisDatabaseContext<TSaga>(database, _options);

            if (correlationId.HasValue && !_options.Optimistic)
                await context.Lock(correlationId.Value, cancellationToken).ConfigureAwait(false);

            return new RedisSagaRepositoryContext<TSaga>(context);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "redis");
        }

        IDatabase GetDatabase()
        {
            return _multiplexer.GetDatabase();
        }
    }
}
