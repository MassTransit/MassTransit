namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Policies;
    using StackExchange.Redis;
    using Util;


    public class RedisDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly IDatabase _database;
        readonly RedisSagaRepositoryOptions<TSaga> _options;

        IAsyncDisposable _lock;

        public RedisDatabaseContext(IDatabase database, RedisSagaRepositoryOptions<TSaga> options)
        {
            _database = database;
            _options = options;
        }

        public async Task Add<T>(SagaConsumeContext<TSaga, T> context)
            where T : class
        {
            var instance = context.Saga;

            try
            {
                await Put(instance).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), typeof(T), instance.CorrelationId, exception);
            }
        }

        public async Task Insert<T>(TSaga instance)
            where T : class
        {
            try
            {
                await Put(instance).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga insert failed", typeof(TSaga), typeof(T), instance.CorrelationId, exception);
            }
        }

        public async Task<TSaga> Load<T>(Guid correlationId)
            where T : class
        {
            try
            {
                return await Get(correlationId).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga load failed", typeof(TSaga), typeof(T), correlationId, exception);
            }
        }

        public async Task Update<T>(SagaConsumeContext<TSaga, T> context)
            where T : class
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
                    throw new RedisSagaConcurrencyException("Saga version conflict", typeof(TSaga), typeof(T), instance.CorrelationId);

                await Put(instance).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), typeof(T), instance.CorrelationId, exception);
            }
            finally
            {
                if (updateLock != null)
                    await updateLock.DisposeAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }

        public async Task Delete<T>(SagaConsumeContext<TSaga, T> context)
            where T : class
        {
            var instance = context.Saga;

            try
            {
                await Delete(instance.CorrelationId).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), typeof(T), instance.CorrelationId, exception);
            }
        }

        public Task DisposeAsync(CancellationToken cancellationToken = default)
        {
            return _lock != null
                ? _lock.DisposeAsync(cancellationToken)
                : TaskUtil.Completed;
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

            return value.IsNullOrEmpty ? null : SagaSerializer.Deserialize<TSaga>(value);
        }

        Task Put(TSaga instance)
        {
            return _database.StringSetAsync(_options.FormatSagaKey(instance.CorrelationId), SagaSerializer.Serialize(instance), _options.Expiry);
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

            public async Task DisposeAsync(CancellationToken cancellationToken)
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

                    throw new MassTransitException($"Unable to lock saga: {TypeMetadataCache<TSaga>.ShortName}({_key})");
                }

                return _options.RetryPolicy.Retry(LockAsync, cancellationToken);
            }
        }
    }
}
