namespace MassTransit.MongoDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
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
        readonly MongoDbCollectionContext<TSaga> _dbContext;
        readonly ISagaConsumeContextFactory<MongoDbCollectionContext<TSaga>, TSaga> _factory;

        public MongoDbSagaRepositoryContext(MongoDbCollectionContext<TSaga> dbContext, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<MongoDbCollectionContext<TSaga>, TSaga> factory)
            : base(consumeContext)
        {
            _dbContext = dbContext;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance,
            SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_dbContext, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_dbContext, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                await _dbContext.InsertOne(instance, CancellationToken).ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_dbContext, _consumeContext, instance, SagaConsumeContextMode.Insert)
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
            FilterDefinition<TSaga> filter = Builders<TSaga>.Filter.Eq(x => x.CorrelationId, correlationId);

            var instance = await _dbContext.Find(filter).FirstOrDefaultAsync(CancellationToken).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_dbContext, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return _dbContext.InsertOne(context.Saga, CancellationToken);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            context.Saga.Version++;

            FilterDefinitionBuilder<TSaga> builder = Builders<TSaga>.Filter;
            FilterDefinition<TSaga> filter = builder.Eq(x => x.CorrelationId, context.Saga.CorrelationId) & builder.Lt(x => x.Version, context.Saga.Version);

            var result = await _dbContext.FindOneAndReplace(filter, context.Saga, CancellationToken).ConfigureAwait(false);
            if (result == null)
                throw new MongoDbConcurrencyException("Saga Update Failed", typeof(TSaga), context.Saga.CorrelationId);
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            FilterDefinitionBuilder<TSaga> builder = Builders<TSaga>.Filter;
            FilterDefinition<TSaga> filter = builder.Eq(x => x.CorrelationId, context.Saga.CorrelationId) & builder.Lte(x => x.Version, context.Saga.Version);

            var result = await _dbContext.DeleteOne(filter, CancellationToken).ConfigureAwait(false);
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
        QuerySagaRepositoryContext<TSaga>,
        LoadSagaRepositoryContext<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly MongoDbCollectionContext<TSaga> _dbContext;

        public MongoDbSagaRepositoryContext(MongoDbCollectionContext<TSaga> dbContext, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _dbContext = dbContext;
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            FilterDefinition<TSaga> filter = Builders<TSaga>.Filter.Eq(x => x.CorrelationId, correlationId);

            return _dbContext.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            IList<Guid> instances = await _dbContext.Find(query.FilterExpression)
                .Project(x => x.CorrelationId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new DefaultSagaRepositoryQueryContext<TSaga>(this, instances);
        }
    }
}
