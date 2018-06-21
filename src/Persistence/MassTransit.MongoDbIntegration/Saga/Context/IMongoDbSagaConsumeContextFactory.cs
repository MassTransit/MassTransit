namespace MassTransit.MongoDbIntegration.Saga.Context
{
    using MongoDB.Driver;


    public interface IMongoDbSagaConsumeContextFactory
    {
        SagaConsumeContext<TSaga, TMessage> Create<TSaga, TMessage>(IMongoCollection<TSaga> collection, ConsumeContext<TMessage> message, TSaga instance, bool existing = true) 
            where TSaga : class, IVersionedSaga 
            where TMessage : class;
    }
}