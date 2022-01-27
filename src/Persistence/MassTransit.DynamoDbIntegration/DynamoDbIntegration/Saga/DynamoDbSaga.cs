namespace MassTransit.DynamoDbIntegration.Saga
{
    using System.Collections.Generic;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.DynamoDBv2.DocumentModel;


    public class DynamoDbSaga
    {
        [DynamoDBIgnore] public static readonly string DefaultEntityType = "SAGA";

        public DynamoDbSaga()
        {
            EntityType = DefaultEntityType;
        }

        [DynamoDBHashKey(AttributeName = "PK")]
        public string CorrelationId { get; set; }

        [DynamoDBRangeKey(AttributeName = "SK")]
        public string EntityType { get; set; } = DefaultEntityType;

        public int VersionNumber { get; set; }

        public string Properties { get; set; }

        public long? ExpirationEpochSeconds { get; set; }

        public Document ToDocument()
        {
            return new Document(new Dictionary<string, DynamoDBEntry>
            {
                { "PK", new Primitive(CorrelationId) },
                { "SK", new Primitive(DefaultEntityType) },
                { nameof(VersionNumber), new Primitive(VersionNumber.ToString(), true) },
                { nameof(Properties), new Primitive(Properties) },
                { nameof(ExpirationEpochSeconds), new Primitive(ExpirationEpochSeconds?.ToString(), true) }
            });
        }
    }
}
