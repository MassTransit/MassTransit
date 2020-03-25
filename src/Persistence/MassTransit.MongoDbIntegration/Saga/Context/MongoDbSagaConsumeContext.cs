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
        readonly IMongoCollection<TSaga> _mongoCollection;
        readonly SagaConsumeContextMode _mode;
        bool _isCompleted;

        public MongoDbSagaConsumeContext(IMongoCollection<TSaga> mongoCollection, ConsumeContext<TMessage> context, TSaga instance,
            SagaConsumeContextMode mode)
            : base(context)
        {
            Saga = instance;
            _mongoCollection = mongoCollection;
            _mode = mode;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            if (_mode == SagaConsumeContextMode.Insert || _mode == SagaConsumeContextMode.Load)
            {
                await Delete().ConfigureAwait(false);

                this.LogRemoved();
            }

            _isCompleted = true;
        }

        async Task Add()
        {
            try
            {
                await _mongoCollection.InsertOneAsync(Saga, null, CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga add failed", typeof(TSaga), typeof(TMessage), Saga.CorrelationId, exception);
            }
        }

        async Task Update()
        {
            Saga.Version++;
            try
            {
                var result = await _mongoCollection.FindOneAndReplaceAsync(x => x.CorrelationId == Saga.CorrelationId && x.Version < Saga.Version,
                    Saga, cancellationToken: CancellationToken).ConfigureAwait(false);

                if (result == null)
                    throw new MongoDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), typeof(TMessage), Saga.CorrelationId, exception);
            }
        }

        async Task Delete()
        {
            try
            {
                var result = await _mongoCollection.DeleteOneAsync(x => x.CorrelationId == Saga.CorrelationId && x.Version <= Saga.Version, CancellationToken)
                    .ConfigureAwait(false);

                if (result.DeletedCount == 0)
                    throw new MongoDbConcurrencyException("Unable to delete saga. It may not have been found or may have been updated.");
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), typeof(TMessage), Saga.CorrelationId, exception);
            }
        }

        public TSaga Saga { get; }

        public bool IsCompleted => _isCompleted;

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return IsCompleted
                ? TaskUtil.Completed
                : _mode == SagaConsumeContextMode.Add
                    ? Add()
                    : Update();
        }
    }
}
