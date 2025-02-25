namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using MongoDB.Bson;
    using MongoDB.Driver;


    public static class SagaRepository
    {
        public static IMongoDatabase Instance => new MongoClient("mongodb://127.0.0.1").GetDatabase("sagaTest");

        public static Task InsertSaga(SimpleSaga saga)
        {
            return Instance.GetCollection<SimpleSaga>("sagas").InsertOneAsync(saga);
        }

        public static Task DeleteSaga(Guid correlationId)
        {
            return Instance.GetCollection<BsonDocument>("sagas").DeleteOneAsync(x => x["_id"].AsGuid == correlationId);
        }

        public static Task<SimpleSaga> GetSaga(Guid correlationId)
        {
            return Instance.GetCollection<SimpleSaga>("sagas").Find(x => x.CorrelationId == correlationId).SingleOrDefaultAsync();
        }
    }
}
