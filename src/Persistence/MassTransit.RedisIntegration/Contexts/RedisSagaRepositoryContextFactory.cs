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

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "redis");
        }

        IDatabase GetDatabase()
        {
            return _multiplexer.GetDatabase();
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

                var sagaRepositoryContext = new RedisSagaRepositoryContext<TSaga, T>(databaseContext, context, _factory);

                await next.Send(sagaRepositoryContext).ConfigureAwait(false);
            }
            finally
            {
                await databaseContext.DisposeAsync().ConfigureAwait(false);
            }
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
            where T : class
        {
            var database = _databaseFactory();

            var databaseContext = new RedisDatabaseContext<TSaga>(database, _options);
            try
            {
                var sagaRepositoryContext = new RedisSagaRepositoryContext<TSaga>(databaseContext, cancellationToken);

                return await asyncMethod(sagaRepositoryContext).ConfigureAwait(false);
            }
            finally
            {
                await databaseContext.DisposeAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}
