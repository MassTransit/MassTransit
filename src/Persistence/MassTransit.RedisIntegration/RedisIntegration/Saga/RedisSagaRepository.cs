namespace MassTransit.RedisIntegration.Saga
{
    using System;
    using MassTransit.Saga;
    using StackExchange.Redis;


    public static class RedisSagaRepository<TSaga>
        where TSaga : class, ISagaVersion
    {
        public static ISagaRepository<TSaga> Create(Func<IDatabase> redisDbFactory, bool optimistic = true, TimeSpan? lockTimeout = null, TimeSpan?
            lockRetryTimeout = null, string keyPrefix = "", TimeSpan? expiry = null)
        {
            var options = new RedisSagaRepositoryOptions<TSaga>(optimistic ? ConcurrencyMode.Optimistic : ConcurrencyMode.Pessimistic, lockTimeout, null,
                keyPrefix, SelectDefaultDatabase, expiry);

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryContextFactory = new RedisSagaRepositoryContextFactory<TSaga>(redisDbFactory, consumeContextFactory, options);

            return new SagaRepository<TSaga>(repositoryContextFactory);
        }

        static IDatabase SelectDefaultDatabase(IConnectionMultiplexer multiplexer)
        {
            return multiplexer.GetDatabase();
        }
    }
}
