namespace MassTransit.MongoDbIntegration.Saga.Context
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Context;
    using MongoDB.Driver;
    using Util;


    public class MongoDbSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
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

                LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                    Saga.CorrelationId, TypeMetadataCache<TMessage>.ShortName);
            }
        }

        public TSaga Saga { get; }

        public bool IsCompleted { get; private set; }
    }
}
