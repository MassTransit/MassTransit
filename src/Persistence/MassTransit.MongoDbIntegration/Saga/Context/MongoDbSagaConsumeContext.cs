namespace MassTransit.MongoDbIntegration.Saga.Context
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;
    using MassTransit.Context;
    using MassTransit.Saga;
    using MongoDB.Driver;


    public class MongoDbSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>,
        IAsyncDisposable
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

        public Task SetCompleted()
        {
            IsCompleted = true;

            return _existing
                ? Remove()
                : TaskUtil.Completed;
        }

        public TSaga Saga { get; }

        public bool IsCompleted { get; private set; }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return IsCompleted
                ? TaskUtil.Completed
                : Add();
        }

        async Task Add()
        {
            try
            {
                await _collection.InsertOneAsync(Saga, null, CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new MongoDbConcurrencyException("Failed to add saga, it may have already been added.", exception);
            }
        }

        async Task Remove()
        {
            var result = await _collection.DeleteOneAsync(x => x.CorrelationId == Saga.CorrelationId && x.Version <= Saga.Version, CancellationToken)
                .ConfigureAwait(false);

            if (result.DeletedCount == 0)
                throw new MongoDbConcurrencyException("Unable to delete saga. It may not have been found or may have been updated.");

            this.LogRemoved();
        }
    }
}
