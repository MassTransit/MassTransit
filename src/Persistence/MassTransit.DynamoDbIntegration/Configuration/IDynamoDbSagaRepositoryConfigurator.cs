namespace MassTransit
{
    using System;
    using Amazon.DynamoDBv2.DataModel;


    public interface IDynamoDbSagaRepositoryConfigurator
    {
        string TableName { set; }
        string LockSuffix { set; }
        TimeSpan LockTimeout { set; }
        TimeSpan? Expiration { set; }

        /// <summary>
        /// Factory method to get the DynamoDb context
        /// </summary>
        /// <param name="contextFactory"></param>
        void ContextFactory(Func<IDynamoDBContext> contextFactory);

        /// <summary>
        /// Use the container to build the DynamoDb context
        /// </summary>
        /// <param name="contextFactory"></param>
        void ContextFactory(Func<IServiceProvider, IDynamoDBContext> contextFactory);
    }


    public interface IDynamoDbSagaRepositoryConfigurator<TSaga> :
        IDynamoDbSagaRepositoryConfigurator
        where TSaga : class, ISagaVersion
    {
    }
}
