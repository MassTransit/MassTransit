namespace MassTransit.DynamoDbIntegration.Saga
{
    using System;
    using Amazon.DynamoDBv2.DataModel;
    using MassTransit.Saga;


    public static class DynamoDbSagaRepository<TSaga>
        where TSaga : class, ISagaVersion
    {
        public static ISagaRepository<TSaga> Create(Func<IDynamoDBContext> dynamoDbFactory, string tableName, TimeSpan? expiration = null)
        {
            var options = new DynamoDbSagaRepositoryOptions<TSaga>(tableName, expiration);

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryContextFactory = new DynamoDbSagaRepositoryContextFactory<TSaga>(dynamoDbFactory, consumeContextFactory, options);

            return new SagaRepository<TSaga>(repositoryContextFactory);
        }
    }
}
