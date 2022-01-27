namespace MassTransit.DynamoDb
{
    using System;
    using Amazon.DynamoDBv2.DataModel;
    using Contexts;
    using Saga;


    public static class DynamoDbSagaRepository<TSaga>
        where TSaga : class, ISagaVersion
    {
        public static ISagaRepository<TSaga> Create(Func<IDynamoDBContext> dynamoDbFactory,string tableName,string lockSuffix = null, TimeSpan? expiry = null)
        {
            var options = new DynamoDbSagaRepositoryOptions<TSaga>(tableName,lockSuffix, expiry);

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryContextFactory = new DynamoDbSagaRepositoryContextFactory<TSaga>(dynamoDbFactory, consumeContextFactory, options);

            return new SagaRepository<TSaga>(repositoryContextFactory);
        }
    }
}
