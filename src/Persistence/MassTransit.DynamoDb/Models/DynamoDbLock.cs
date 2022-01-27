namespace MassTransit.DynamoDb.Models
{
    using System;
    using Amazon.DynamoDBv2.DataModel;


    public class DynamoDbLock
    {
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

        [DynamoDBIgnore] public static string DefaultEntityType = "LOCK";


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
