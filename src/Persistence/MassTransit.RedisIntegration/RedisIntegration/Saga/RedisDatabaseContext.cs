namespace MassTransit.RedisIntegration.Saga
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Metadata;
    using RetryPolicies;
    using Serialization;
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

            return Put(instance);
        }

        public Task Insert(TSaga instance)
        {
            return Put(instance);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return Get(correlationId);
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

                var existingInstance = await Get(instance.CorrelationId).ConfigureAwait(false);
                if (existingInstance.Version >= instance.Version)
                    throw new RedisSagaConcurrencyException("Saga version conflict", typeof(TSaga), instance.CorrelationId);

                await Put(instance).ConfigureAwait(false);
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

        public async Task<TSaga> Get(Guid correlationId)
        {
            var value = await _database.StringGetAsync(_options.FormatSagaKey(correlationId)).ConfigureAwait(false);

            return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<TSaga>(value, SystemTextJsonMessageSerializer.Options);
        }

        Task Put(TSaga instance)
        {
            return _database.StringSetAsync(_options.FormatSagaKey(instance.CorrelationId),
                JsonSerializer.Serialize(instance, SystemTextJsonMessageSerializer.Options), _options.Expiry);
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

                return _options.RetryPolicy.Retry(LockAsync, cancellationToken);
            }
        }
    }
}
