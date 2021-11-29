namespace MassTransit.AzureTable.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;
    using Middleware;


    public class AzureTableSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly DatabaseContext<TSaga> _context;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public AzureTableSagaRepositoryContext(DatabaseContext<TSaga> context, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
            : base(consumeContext)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                var result = await TableInsert(instance).ConfigureAwait(false);
                if (result.Result is DynamicTableEntity tableEntity)
                {
                    _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                    return await CreateSagaConsumeContext(tableEntity, SagaConsumeContextMode.Insert).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);
            }

            return default;
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var (partitionKey, rowKey) = _context.Formatter.Format(correlationId);

            var operation = TableOperation.Retrieve<DynamicTableEntity>(partitionKey, rowKey);
            var result = await _context.Table.ExecuteAsync(operation, CancellationToken).ConfigureAwait(false);

            if (result.Result is DynamicTableEntity tableEntity)
                return await CreateSagaConsumeContext(tableEntity, SagaConsumeContextMode.Load).ConfigureAwait(false);

            return default;
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return TableInsert(context.Saga);
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            try
            {
                IDictionary<string, EntityProperty> entityProperties = _context.Converter.GetDictionary(instance);

                var (partitionKey, rowKey) = _context.Formatter.Format(instance.CorrelationId);

                var eTag = context.GetPayload<SagaETag>();

                var operation = TableOperation.Replace(new DynamicTableEntity(partitionKey, rowKey)
                {
                    Properties = entityProperties,
                    ETag = eTag.ETag
                });

                await _context.Table.ExecuteAsync(operation, context.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), instance.CorrelationId, exception);
            }
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            IDictionary<string, EntityProperty> entityProperties = _context.Converter.GetDictionary(instance);

            var (partitionKey, rowKey) = _context.Formatter.Format(instance.CorrelationId);

            var eTag = context.GetPayload<SagaETag>();

            var operation = TableOperation.Delete(new DynamicTableEntity(partitionKey, rowKey, eTag.ETag, entityProperties));

            await _context.Table.ExecuteAsync(operation, context.CancellationToken).ConfigureAwait(false);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_context, consumeContext, instance, mode);
        }

        Task<TableResult> TableInsert(TSaga instance)
        {
            IDictionary<string, EntityProperty> entityProperties = _context.Converter.GetDictionary(instance);

            var (partitionKey, rowKey) = _context.Formatter.Format(instance.CorrelationId);

            var operation = TableOperation.Insert(new DynamicTableEntity(partitionKey, rowKey) {Properties = entityProperties});

            return _context.Table.ExecuteAsync(operation, CancellationToken);
        }

        async Task<SagaConsumeContext<TSaga, TMessage>> CreateSagaConsumeContext(DynamicTableEntity entity, SagaConsumeContextMode mode)
        {
            var instance = _context.Converter.GetObject(entity.Properties);

            SagaConsumeContext<TSaga, TMessage> sagaConsumeContext = await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, mode)
                .ConfigureAwait(false);

            var eTag = new SagaETag(entity.ETag);

            sagaConsumeContext.AddOrUpdatePayload(() => eTag, _ => eTag);

            return sagaConsumeContext;
        }
    }


    public class CosmosTableSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DatabaseContext<TSaga> _context;

        public CosmosTableSagaRepositoryContext(DatabaseContext<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedByDesignException("Azure Table saga repository does not support queries");
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            var (partitionKey, rowKey) = _context.Formatter.Format(correlationId);

            var operation = TableOperation.Retrieve<DynamicTableEntity>(partitionKey, rowKey);
            var result = await _context.Table.ExecuteAsync(operation, CancellationToken).ConfigureAwait(false);
            if (result.Result is DynamicTableEntity entity)
                return _context.Converter.GetObject(entity.Properties);


            return default;
        }
    }
}
