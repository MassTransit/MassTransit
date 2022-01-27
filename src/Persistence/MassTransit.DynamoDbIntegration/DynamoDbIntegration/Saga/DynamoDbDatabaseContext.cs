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
            return Save(context.Saga);
        }

        public Task Insert(TSaga instance)
        {
            return Save(instance);
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            var value = await _database.LoadAsync<DynamoDbSaga>(_options.FormatSagaKey(correlationId), DynamoDbSaga.DefaultEntityType, _options.Config);
            return value == null
                ? null
                : JsonSerializer.Deserialize<TSaga>(value.Properties, SystemTextJsonMessageSerializer.Options);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;
            try
            {
                var operationConfig = BuildUpdateItemOperationConfig(instance.Version);

                ++instance.Version;

                var updateSaga = GetDynamoDbSaga(instance);

                await _database.GetTargetTable<DynamoDbSaga>(_options.Config)
                    .UpdateItemAsync(updateSaga.ToDocument(), new Primitive(updateSaga.CorrelationId), new Primitive(DynamoDbSaga.DefaultEntityType),
                        operationConfig);
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
            return _database.DeleteAsync(new DynamoDbSaga { CorrelationId = _options.FormatSagaKey(context.Saga.CorrelationId) }, _options.Config);
        }

        public void Dispose()
        {
            _database?.Dispose();
        }

        DynamoDbSaga GetDynamoDbSaga(TSaga instance)
        {
            return new DynamoDbSaga
            {
                CorrelationId = _options.FormatSagaKey(instance.CorrelationId),
                VersionNumber = instance.Version,
                Properties = JsonSerializer.Serialize(instance, SystemTextJsonMessageSerializer.Options),
                ExpirationEpochSeconds = GetExpirationEpochSeconds()
            };
        }

        long? GetExpirationEpochSeconds()
        {
            return _options.Expiration.HasValue
                ? DateTimeOffset.UtcNow.Add(_options.Expiration.Value).ToUnixTimeSeconds()
                : (long?)null;
        }

        async Task Save(TSaga instance)
        {
            try
            {
                var addSaga = GetDynamoDbSaga(instance);

                var operationConfig = BuildPutItemOperationConfig(addSaga.CorrelationId);

                await _database.GetTargetTable<DynamoDbSaga>(_options.Config).PutItemAsync(addSaga.ToDocument(), operationConfig);
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

        static UpdateItemOperationConfig BuildUpdateItemOperationConfig(int expectedVersion)
        {
            return new UpdateItemOperationConfig
            {
                ConditionalExpression = new Expression
                {
                    ExpressionStatement = "VersionNumber = :versionNumber",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> { { ":versionNumber", new Primitive(expectedVersion.ToString(), true) } }
                }
            };
        }

        static PutItemOperationConfig BuildPutItemOperationConfig(string correlationId)
        {
            return new PutItemOperationConfig
            {
                ConditionalExpression = new Expression
                {
                    ExpressionStatement = "CorrelationId <> :correlationId AND EntityType <> :entityType",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":correlationId", new Primitive(correlationId) },
                        { ":entityType", new Primitive("SAGA") }
                    }
                }
            };
        }
    }
}
