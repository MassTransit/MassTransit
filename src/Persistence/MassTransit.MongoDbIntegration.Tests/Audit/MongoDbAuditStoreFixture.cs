namespace MassTransit.MongoDbIntegration.Tests.Audit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Audit;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using MongoDbIntegration.Audit;


    public static class MongoDbAuditStoreFixture
    {
        static MongoDbAuditStoreFixture()
        {
            Database = new MongoClient("mongodb://127.0.0.1").GetDatabase("auditTest");

            AuditCollectionName = Guid.NewGuid().ToString("N");
            AuditStore = new MongoDbAuditStore(Database, AuditCollectionName);
            AuditCollection = Database.GetCollection<AuditDocument>(AuditCollectionName);
        }

        public static string AuditCollectionName { get; }
        public static IMongoDatabase Database { get; }
        public static IMessageAuditStore AuditStore { get; }
        public static IMongoCollection<AuditDocument> AuditCollection { get; }

        public static Task<List<AuditDocument>> GetAuditRecords(string contextType)
        {
            return Database.GetCollection<AuditDocument>(AuditCollectionName)
                .AsQueryable()
                .Where(x => x.ContextType == contextType)
                .ToListAsync();
        }

        public static Task Cleanup()
        {
            return AuditCollection.DeleteManyAsync(new FilterDefinitionBuilder<AuditDocument>().Empty);
        }
    }
}
