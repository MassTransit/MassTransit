namespace MassTransit.DynamoDbIntegration.Saga
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2.DataModel;
    using Metadata;
    using RetryPolicies;
    using Serialization;


    public class DynamoDbDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly IDynamoDBContext _database;
        readonly DynamoDbSagaRepositoryOptions<TSaga> _options;
        IAsyncDisposable _lock;

        public DynamoDbDatabaseContext(IDynamoDBContext database, DynamoDbSagaRepositoryOptions<TSaga> options)
        {
            _database = database;
            _options = options;
        }

        public Task Add(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            return Save(instance);
        }

        public Task Insert(TSaga instance)
        {
            return Save(instance);
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            var value = await _database.LoadAsync<DynamoDbSaga>(_options.FormatSagaKey(correlationId), DynamoDbSaga.DefaultEntityType,
                _options.DefaultConfig());
            return value == null ? null : JsonSerializer.Deserialize<TSaga>(value.Properties, SystemTextJsonMessageSerializer.Options);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;
            var updateLock = await Lock(instance, context.CancellationToken).ConfigureAwait(false);
            try
            {
                instance.Version++;

                var existingInstance = await Load(instance.CorrelationId).ConfigureAwait(false);
                if (existingInstance.Version >= instance.Version)
                    throw new DynamoDbSagaConcurrencyException("Saga version conflict", typeof(TSaga), instance.CorrelationId);

                await Save(instance).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), instance.CorrelationId, exception);
            }
            finally
            {
                updateLock?.DisposeAsync().ConfigureAwait(false);
            }
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _database.DeleteAsync(new DynamoDbSaga { CorrelationId = _options.FormatSagaKey(context.Saga.CorrelationId) },
                _options.DefaultConfig());
        }

        public ValueTask DisposeAsync()
        {
            _database?.Dispose();
            return _lock?.DisposeAsync() ?? default;
        }

        async Task Save(TSaga instance)
        {
            await _database.SaveAsync(new DynamoDbSaga
            {
                CorrelationId = _options.FormatSagaKey(instance.CorrelationId),
                VersionNumber = instance.Version,
                Properties = JsonSerializer.Serialize(instance, SystemTextJsonMessageSerializer.Options),
                ExpirationEpochSeconds = _options.Expiration.HasValue ? DateTimeOffset.UtcNow.Add(_options.Expiration.Value).ToUnixTimeSeconds() : (long?)null
            }, _options.DefaultConfig());
        }

        public async Task Lock(Guid correlationId, CancellationToken cancellationToken)
        {
            if (_lock != null)
                throw new InvalidOperationException("The database context is already locked");

            var sagaLock = new DynamoDbLock(_database, _options, correlationId);

            _lock = await sagaLock.Lock(cancellationToken).ConfigureAwait(false);
        }

        Task<IAsyncDisposable> Lock(TSaga instance, CancellationToken cancellationToken)
        {
            var sagaLock = new DynamoDbLock(_database, _options, instance.CorrelationId);

            return sagaLock.Lock(cancellationToken);
        }


        class DynamoDbLock :
            IAsyncDisposable
        {
            readonly IDynamoDBContext _db;
            readonly string _key;
            readonly DynamoDbSagaRepositoryOptions<TSaga> _options;
            readonly string _token;

            public DynamoDbLock(IDynamoDBContext db, DynamoDbSagaRepositoryOptions<TSaga> options, Guid correlationId)
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
                    await _db.DeleteAsync<Saga.DynamoDbLock>(_key, Saga.DynamoDbLock.DefaultEntityType, _options.DefaultConfig()).ConfigureAwait(false);
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
                    var result = await _db
                        .LoadAsync<Saga.DynamoDbLock>(_key, Saga.DynamoDbLock.DefaultEntityType, _options.DefaultConfig(), cancellationToken)
                        .ConfigureAwait(false);

                    if (result != null && result.GetLockedUntilEpoch() > DateTimeOffset.UtcNow)
                        throw new MassTransitException($"Unable to lock saga: {TypeMetadataCache<TSaga>.ShortName}({_key})");

                    await _db.SaveAsync(new Saga.DynamoDbLock
                    {
                        CorrelationId = _key,
                        Token = _token,
                        LockedUntilEpoch = DateTimeOffset.UtcNow.Add(_options.LockTimeout).ToUnixTimeSeconds()
                    }, _options.DefaultConfig(), cancellationToken);

                    return this;
                }

                return _options.RetryPolicy.Retry(LockAsync, cancellationToken);
            }
        }
    }
}
