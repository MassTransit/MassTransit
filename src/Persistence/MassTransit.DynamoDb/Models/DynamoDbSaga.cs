namespace MassTransit.DynamoDb.Models
{
    using Amazon.DynamoDBv2.DataModel;


    public class DynamoDbSaga
    {
        public DynamoDbSaga()
        {
            EntityType = DefaultEntityType;
        }

        [DynamoDBHashKey(AttributeName = "PK")]
        public string CorrelationId { get; set; }

        [DynamoDBRangeKey(AttributeName = "SK")]
        public string EntityType { get; set; } = "SAGA";

        public int? VersionNumber { get; set; }

        public string Properties { get; set; }

        public long? ExpirationEpochSeconds { get; set; }

        [DynamoDBIgnore] public static string DefaultEntityType = "SAGA";
    }
}
