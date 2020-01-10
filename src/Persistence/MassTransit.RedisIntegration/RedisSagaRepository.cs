namespace MassTransit.RedisIntegration
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Saga;
    using Metadata;
    using StackExchange.Redis;


    public class RedisSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IRetrieveSagaFromRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly ISagaRepository<TSaga> _repository;
        readonly RedisSagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public RedisSagaRepository(Func<IDatabase> redisDbFactory, bool optimistic = true, TimeSpan? lockTimeout = null, TimeSpan? lockRetryTimeout = null,
            string keyPrefix = "")
        {
            var options = new RedisSagaRepositoryOptions<TSaga>(optimistic, lockTimeout, null, keyPrefix);

            var consumeContextFactory = new RedisSagaConsumeContextFactory<TSaga>();

            _repositoryContextFactory = new RedisSagaRepositoryContextFactory<TSaga>(redisDbFactory, consumeContextFactory, options);

            _repository = new SagaRepository<TSaga>(_repositoryContextFactory);
        }

        public async Task<TSaga> GetSaga(Guid correlationId)
        {
            SagaRepositoryContext<TSaga> repositoryContext = await _repositoryContextFactory.CreateContext().ConfigureAwait(false);
            try
            {
                if (repositoryContext is RedisSagaRepositoryContext<TSaga> context && context.Context is RedisDatabaseContext<TSaga> databaseContext)
                    return await databaseContext.Get(correlationId).ConfigureAwait(false);
            }
            finally
            {
                switch (repositoryContext)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }

            throw new NotSupportedException($"{nameof(GetSaga)} is not supported for {TypeMetadataCache<TSaga>.ShortName}");
        }

        Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.Send(context, policy, next);
        }

        Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.SendQuery(context, policy, next);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _repository.Probe(context);
        }
    }
}
