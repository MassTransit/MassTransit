namespace MassTransit.MongoDbIntegration.Saga.Context
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Context;
    using MassTransit.Saga;
    using MongoDB.Driver;


    public class MongoDbSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, IVersionedSaga
    {
        readonly IMongoCollection<TSaga> _collection;
        readonly bool _existing;

        public MongoDbSagaConsumeContext(IMongoCollection<TSaga> collection, ConsumeContext<TMessage> context, TSaga instance, bool existing = true)
            : base(context)
        {
            Saga = instance;
            _collection = collection;
            _existing = existing;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            IsCompleted = true;

            if (_existing)
            {
                var result = await _collection.DeleteOneAsync(x => x.CorrelationId == Saga.CorrelationId && x.Version <= Saga.Version, CancellationToken)
                    .ConfigureAwait(false);

                if (result.DeletedCount == 0)
                    throw new MongoDbConcurrencyException("Unable to delete saga. It may not have been found or may have been updated by another process.");

                this.LogRemoved();
            }
        }

        public TSaga Saga { get; }

        public bool IsCompleted { get; private set; }
    }
}
