namespace MassTransit.RedisIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;
    using RetryPolicies;
    using StackExchange.Redis;


    public class RedisDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly IDatabase _database;
        readonly RedisSagaRepositoryOptions<TSaga> _options;

        IAsyncDisposable _lock;

        public RedisDatabaseContext(IDatabase database, RedisSagaRepositoryOptions<TSaga> options)
        {
            _database = database;
            _options = options;
        }

        public Task Add(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            return Put(context.SerializerContext.SerializeObject(instance).GetString(), instance.CorrelationId);
        }

        public Task Insert(IObjectDeserializer deserializer, TSaga instance)
        {
            return Put(deserializer.SerializeObject(instance).GetString(), instance.CorrelationId);
        }

        public Task<TSaga> Load(IObjectDeserializer deserializer, Guid correlationId)
        {
            return Get(deserializer, correlationId);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            IAsyncDisposable updateLock = _options.ConcurrencyMode == ConcurrencyMode.Optimistic
                ? updateLock = await Lock(instance, context.CancellationToken).ConfigureAwait(false)
                : null;

            try
            {
                instance.Version++;

                var deserializer = context.SerializerContext;

                var existingInstance = await Get(deserializer, instance.CorrelationId).ConfigureAwait(false);
                if (existingInstance.Version >= instance.Version)
                    throw new RedisSagaConcurrencyException("Saga version conflict", typeof(TSaga), instance.CorrelationId);

                await Put(deserializer.SerializeObject(instance).GetString(), instance.CorrelationId).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), instance.CorrelationId, exception);
            }
            finally
            {
                if (updateLock != null)
                    await updateLock.DisposeAsync().ConfigureAwait(false);
            }
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return Delete(context.Saga.CorrelationId);
        }

        public ValueTask DisposeAsync()
        {
            return _lock?.DisposeAsync() ?? default;
        }

        /// <summary>
        /// Add a pessimistic lock to the database context
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Lock(Guid correlationId, CancellationToken cancellationToken)
        {
            if (_lock != null)
                throw new InvalidOperationException("The database context is already locked");

            var sagaLock = new SagaLock(_database, _options, correlationId);

            _lock = await sagaLock.Lock(cancellationToken).ConfigureAwait(false);
        }

        Task<IAsyncDisposable> Lock(TSaga instance, CancellationToken cancellationToken)
        {
            var sagaLock = new SagaLock(_database, _options, instance.CorrelationId);

            return sagaLock.Lock(cancellationToken);
        }

        async Task<TSaga> Get(IObjectDeserializer deserializer, Guid correlationId)
        {
            var value = await _database.StringGetAsync(_options.FormatSagaKey(correlationId)).ConfigureAwait(false);

            return value.IsNullOrEmpty ? null : deserializer.DeserializeObject<TSaga>(value.ToString());
        }

        Task Put(string instance, Guid correlationId)
        {
            return _database.StringSetAsync(_options.FormatSagaKey(correlationId), instance, _options.Expiry);
        }

        Task Delete(Guid correlationId)
        {
            return _database.KeyDeleteAsync(_options.FormatSagaKey(correlationId));
        }


        class SagaLock :
            IAsyncDisposable
        {
            readonly IDatabase _db;
            readonly RedisKey _key;
            readonly RedisSagaRepositoryOptions<TSaga> _options;
            readonly RedisValue _token;

            public SagaLock(IDatabase db, RedisSagaRepositoryOptions<TSaga> options, Guid correlationId)
            {
                _db = db;
                _options = options;

                _key = _options.FormatLockKey(correlationId);
                _token = $"{HostMetadataCache.Host.MachineName}:{NewId.NextGuid()}";
            }

            public async ValueTask DisposeAsync()
            {
                try
                {
                    await _db.LockReleaseAsync(_key, _token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogContext.Warning?.Log(ex, $"Failed to release lock: {_key}");
                }
            }

            public Task<IAsyncDisposable> Lock(CancellationToken cancellationToken)
            {
                async Task<IAsyncDisposable> LockAsync()
                {
                    var result = await _db.LockTakeAsync(_key, _token, _options.LockTimeout).ConfigureAwait(false);

                    if (result)
                        return this;

                    throw new MassTransitException($"Unable to lock saga: {TypeCache<TSaga>.ShortName}({_key})");
                }

                return _options.RetryPolicy.Retry(LockAsync, false, cancellationToken);
            }
        }
    }
}
