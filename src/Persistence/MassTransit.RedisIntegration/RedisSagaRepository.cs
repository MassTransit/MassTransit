namespace MassTransit.RedisIntegration
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Metadata;
    using Saga;
    using StackExchange.Redis;
    using Util;


    public class RedisSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>,
        IRetrieveSagaFromRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly ISagaRepository<TSaga> _repository;
        readonly RedisSagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public RedisSagaRepository(Func<IDatabase> redisDbFactory, bool optimistic = true, TimeSpan? lockTimeout = null, TimeSpan? lockRetryTimeout = null,
            string keyPrefix = "")
        {
            var options = new RedisSagaRepositoryOptions<TSaga>(optimistic ? ConcurrencyMode.Optimistic : ConcurrencyMode.Pessimistic, lockTimeout, null,
                keyPrefix);

            var consumeContextFactory = new RedisSagaConsumeContextFactory<TSaga>();

            _repositoryContextFactory = new RedisSagaRepositoryContextFactory<TSaga>(redisDbFactory, consumeContextFactory, options);

            _repository = new SagaRepository<TSaga>(_repositoryContextFactory);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _repositoryContextFactory.Execute(context =>
            {
                if (context is RedisSagaRepositoryContext<TSaga> redisSagaRepositoryContext
                    && redisSagaRepositoryContext.Context is RedisDatabaseContext<TSaga> databaseContext)
                    return databaseContext.Get(correlationId);

                return TaskUtil.Faulted<TSaga>(new NotSupportedException(
                    $"{nameof(GetSaga)} is not supported for {TypeMetadataCache<TSaga>.ShortName}"));
            });
        }

        public Task<TSaga> GetSaga(Guid correlationId)
        {
            return Load(correlationId);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _repository.Probe(context);
        }

        Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.Send(context, policy, next);
        }

        Task ISagaRepository<TSaga>.SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.SendQuery(context, query, policy, next);
        }
    }
}
