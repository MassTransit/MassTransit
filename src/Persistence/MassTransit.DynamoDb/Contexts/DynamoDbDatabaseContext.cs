namespace MassTransit.DynamoDb.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2.DataModel;
    using DynamoDb;
    using Exceptions;
    using Models;
    using Saga;


    public class DynamoDbDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly IDynamoDBContext _database;
        readonly DynamoDbSagaRepositoryOptions<TSaga> _options;

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

        async Task Save(TSaga instance)
        {
            await _database.SaveAsync(new DynamoDbSaga
            {
                CorrelationId = _options.FormatSagaKey(instance.CorrelationId),
                VersionNumber = instance.Version,
                Properties = DynamoDb.SagaSerializer.Serialize(instance),
                ExpirationEpochSeconds = _options.Expiry.HasValue ? DateTimeOffset.UtcNow.Add(_options.Expiry.Value).ToUnixTimeSeconds() : (long?)null
            }, _options.DefaultConfig());
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            var value = await _database.LoadAsync<DynamoDbSaga>(_options.FormatSagaKey(correlationId),
                _options.DefaultConfig());
            return value == null ? null : DynamoDb.SagaSerializer.Deserialize<TSaga>(value.Properties);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;
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
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _database.DeleteAsync(_options.FormatSagaKey(context.Saga.CorrelationId),
                _options.DefaultConfig());
        }

        public ValueTask DisposeAsync()
        {
            _database.Dispose();
            return new ValueTask(Task.CompletedTask);
        }
    }
}
