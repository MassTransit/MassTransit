namespace MassTransit
{
    using System;
    using System.Text;
    using Amazon.DynamoDBv2.DataModel;


    public class DynamoDbSagaRepositoryOptions<TSaga>
        where TSaga : class, ISaga
    {
        public DynamoDbSagaRepositoryOptions(string tableName, string lockSuffix, TimeSpan lockTimeout, TimeSpan? sagaExpiration)
        {
            TableName = tableName;
            LockSuffix = lockSuffix;
            LockTimeout = lockTimeout;
            Expiration = sagaExpiration;
            RetryPolicy = Retry.Exponential(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(918));
        }

        public IRetryPolicy RetryPolicy { get; }
        public string TableName { get; }
        public string LockSuffix { get; }
        public TimeSpan LockTimeout { get; }
        public TimeSpan? Expiration { get; }

        public string FormatSagaKey(Guid correlationId)
        {
            return correlationId.ToString();
        }

        public string FormatLockKey(Guid correlationId)
        {
            var sb = new StringBuilder(correlationId.ToString());
            if (!string.IsNullOrWhiteSpace(LockSuffix))
                sb.Append(LockSuffix);
            return sb.ToString();
        }

        public DynamoDBOperationConfig DefaultConfig()
        {
            return new DynamoDBOperationConfig { OverrideTableName = TableName };
        }
    }
}
