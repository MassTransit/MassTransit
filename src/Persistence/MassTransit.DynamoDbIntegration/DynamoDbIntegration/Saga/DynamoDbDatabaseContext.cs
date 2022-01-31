namespace MassTransit.DynamoDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Serialization;


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

        public async Task<TSaga> Load(Guid correlationId)
        {
            var value = await _database.LoadAsync<DynamoDbSaga>(_options.FormatSagaKey(correlationId), DynamoDbSaga.DefaultEntityType,
                _options.DefaultConfig());
            return value == null ? null : JsonSerializer.Deserialize<TSaga>(value.Properties, SystemTextJsonMessageSerializer.Options);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;
            try
            {
                var toUpdateSaga = new DynamoDbSaga
                {
                    CorrelationId = _options.FormatSagaKey(instance.CorrelationId),
                    VersionNumber = ++instance.Version,
                    Properties = JsonSerializer.Serialize(instance, SystemTextJsonMessageSerializer.Options),
                    ExpirationEpochSeconds = _options.Expiration.HasValue
                        ? DateTimeOffset.UtcNow.Add(_options.Expiration.Value).ToUnixTimeSeconds()
                        : (long?)null
                };

                await _database.GetTargetTable<DynamoDbSaga>(_options.DefaultConfig())
                    .UpdateItemAsync(toUpdateSaga.ToDocument(), new Primitive(toUpdateSaga.CorrelationId), new Primitive(DynamoDbSaga.DefaultEntityType),
                        BuildVersionNumberCondition(toUpdateSaga.VersionNumber - 1));
            }
            catch (ConditionalCheckFailedException)
            {
                throw new DynamoDbSagaConcurrencyException("Saga version conflict", typeof(TSaga), instance.CorrelationId);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), instance.CorrelationId, exception);
            }
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _database.DeleteAsync(new DynamoDbSaga { CorrelationId = _options.FormatSagaKey(context.Saga.CorrelationId) },
                _options.DefaultConfig());
        }

        public void Dispose()
        {
            _database?.Dispose();
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

        static UpdateItemOperationConfig BuildVersionNumberCondition(int expectedVersion)
        {
            return new UpdateItemOperationConfig
            {
                ConditionalExpression = new Expression
                {
                    ExpressionStatement = "VersionNumber = :versionNumber",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":versionNumber", new Primitive(expectedVersion.ToString(), true)}
                    }
                }
            };
        }
    }
}
