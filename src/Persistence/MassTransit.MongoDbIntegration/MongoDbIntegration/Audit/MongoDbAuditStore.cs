namespace MassTransit.MongoDbIntegration.Audit
{
    using System.Threading.Tasks;
    using MassTransit.Audit;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Options;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;


    public class MongoDbAuditStore : IMessageAuditStore
    {
        readonly IMongoCollection<AuditDocument> _collection;

        static MongoDbAuditStore()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(AuditDocument)))
                return;

            // easiest way to metadata since keys wont become element names, therefore subject to validation
            // will allow keys like $correlationId to be kept
            var headersSerializer = new DictionaryInterfaceImplementerSerializer<AuditHeaders, string, string>(
                DictionaryRepresentation.ArrayOfDocuments
            );

            BsonClassMap.RegisterClassMap<AuditDocument>(x =>
            {
                x.AutoMap();
                x.MapIdMember(doc => doc.AuditId);
                x.MapMember(doc => doc.Headers).SetSerializer(headersSerializer);
                x.MapMember(doc => doc.Custom).SetSerializer(headersSerializer);
            });
        }

        public MongoDbAuditStore(MongoUrl mongoUrl, string collectionName)
            : this(mongoUrl.Url, mongoUrl.DatabaseName, collectionName)
        {
        }

        public MongoDbAuditStore(string connectionString, string database, string collectionName)
            : this(new MongoClient(connectionString).GetDatabase(database), collectionName)
        {
        }

        public MongoDbAuditStore(IMongoDatabase mongoDatabase, string collectionName)
        {
            _collection = mongoDatabase.GetCollection<AuditDocument>(collectionName);
        }

        public Task StoreMessage<T>(T message, MessageAuditMetadata metadata)
            where T : class
        {
            var auditDocument = AuditDocument.Create(message, TypeCache<T>.ShortName, metadata);

            return _collection.InsertOneAsync(auditDocument);
        }
    }
}
