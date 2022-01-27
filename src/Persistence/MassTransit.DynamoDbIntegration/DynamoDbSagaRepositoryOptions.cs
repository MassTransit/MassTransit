namespace MassTransit
{
    using System;
    using Amazon.DynamoDBv2.DataModel;


    public class DynamoDbSagaRepositoryOptions<TSaga>
        where TSaga : class, ISaga
    {
        public DynamoDbSagaRepositoryOptions(string tableName, TimeSpan? expiration)
        {
            Expiration = expiration;
            Config = new DynamoDBOperationConfig { OverrideTableName = tableName };
        }

        public DynamoDBOperationConfig Config { get; private set; }

        public TimeSpan? Expiration { get; }

        public string FormatSagaKey(Guid correlationId)
        {
            return correlationId.ToString();
        }
    }
}
