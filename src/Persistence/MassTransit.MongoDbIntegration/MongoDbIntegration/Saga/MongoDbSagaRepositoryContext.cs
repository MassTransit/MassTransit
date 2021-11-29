namespace MassTransit.MongoDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Context;
    using MassTransit.Saga;
    using Middleware;
    using MongoDB.Driver;


    public class MongoDbSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISagaVersion
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<IMongoCollection<TSaga>, TSaga> _factory;
        readonly IMongoCollection<TSaga> _mongoCollection;

        public MongoDbSagaRepositoryContext(IMongoCollection<TSaga> mongoCollection, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<IMongoCollection<TSaga>, TSaga> factory)
            : base(consumeContext)
        {
            _mongoCollection = mongoCollection;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_mongoCollection, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_mongoCollection, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                await _mongoCollection.InsertOneAsync(instance).ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_mongoCollection, _consumeContext, instance, SagaConsumeContextMode.Insert)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);

                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _mongoCollection.Find(Builders<TSaga>.Filter.Eq(x => x.CorrelationId, correlationId))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_mongoCollection, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return _mongoCollection.InsertOneAsync(context.Saga, null, CancellationToken);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            context.Saga.Version++;
            var result = await _mongoCollection.FindOneAndReplaceAsync(x => x.CorrelationId == context.Saga.CorrelationId && x.Version < context.Saga.Version,
                context.Saga, cancellationToken: CancellationToken).ConfigureAwait(false);

            if (result == null)
                throw new MongoDbConcurrencyException("Saga Update Failed", typeof(TSaga), context.Saga.CorrelationId);
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            var result = await _mongoCollection.DeleteOneAsync(x => x.CorrelationId == context.Saga.CorrelationId && x.Version <= context.Saga.Version,
                CancellationToken).ConfigureAwait(false);

            if (result.DeletedCount == 0)
                throw new MongoDbConcurrencyException("Saga Delete Failed", typeof(TSaga), context.Saga.CorrelationId);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }
    }


    public class MongoDbSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly IMongoCollection<TSaga> _context;

        public MongoDbSagaRepositoryContext(IMongoCollection<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            IList<Guid> instances = await _context.Find(query.FilterExpression)
                .Project(x => x.CorrelationId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new DefaultSagaRepositoryQueryContext<TSaga>(this, instances);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.Find(x => x.CorrelationId == correlationId).FirstOrDefaultAsync();
        }
    }
}
