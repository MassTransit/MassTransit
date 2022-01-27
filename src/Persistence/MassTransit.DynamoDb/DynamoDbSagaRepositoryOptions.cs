namespace MassTransit.DynamoDb
{
    using System;
    using Amazon.DynamoDBv2.DataModel;
    using Saga;


    public class DynamoDbSagaRepositoryOptions<TSaga>
        where TSaga : class, ISaga
    {
        public DynamoDbSagaRepositoryOptions(string tableName, string lockSuffix, TimeSpan? expiry)
        {
            TableName = tableName;
            LockSuffix = lockSuffix;
            Expiry = expiry;
        }

        public string TableName { get; }
        public string LockSuffix { get; }
        public TimeSpan? Expiry { get; }

        public string FormatSagaKey(Guid correlationId)
        {
            return correlationId.ToString();
        }

        public DynamoDBOperationConfig DefaultConfig()
        {
            return new DynamoDBOperationConfig {OverrideTableName = TableName};
        }

        public string FormatLockKey(Guid correlationId)
        {
            return string.IsNullOrWhiteSpace(LockSuffix) ? correlationId.ToString() : $"{correlationId}{LockSuffix}";
        }
    }
}
