namespace MassTransit.DynamoDbIntegration.Saga
{
    using System;
    using Amazon.DynamoDBv2.DataModel;


    public class DynamoDbLock
    {
        [DynamoDBIgnore] public static string DefaultEntityType = "LOCK";

        public DynamoDbLock()
        {
            EntityType = DefaultEntityType;
            LockedAtEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        [DynamoDBHashKey(AttributeName = "PK")]
        public string CorrelationId { get; set; }

        [DynamoDBRangeKey(AttributeName = "SK")]
        public string EntityType { get; set; } = DefaultEntityType;

        public string Token { get; set; }

        [DynamoDBProperty(StoreAsEpoch = true)]
        public long LockedUntilEpoch { get; set; }

        [DynamoDBProperty(StoreAsEpoch = true)]
        public long LockedAtEpoch { get; set; }

        public DateTimeOffset GetLockedUntilEpoch()
        {
            return DateTimeOffset.FromUnixTimeSeconds(LockedUntilEpoch);
        }

        public DateTimeOffset GetLockedAtEpoch()
        {
            return DateTimeOffset.FromUnixTimeSeconds(LockedAtEpoch);
        }
    }
}
