namespace MassTransit.DynamoDb.Models
{
    using Amazon.DynamoDBv2.DataModel;

    public class DynamoDbSaga
    {
        [DynamoDBHashKey]
        public string CorrelationId { get; set; }

        public int? VersionNumber { get; set; }

        public string Properties { get; set; }

        public long? ExpirationEpochSeconds { get; set; }
    }
}
