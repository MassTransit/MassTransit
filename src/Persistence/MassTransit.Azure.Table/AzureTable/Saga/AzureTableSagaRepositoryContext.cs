namespace MassTransit.AzureTable.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using Azure.Data.Tables;
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
                var (insert, entity) = TableInsert(instance);
                var result = await insert.ConfigureAwait(false);
                if (!result.IsError)
                {
                    _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                    return await CreateSagaConsumeContext(entity, SagaConsumeContextMode.Insert).ConfigureAwait(false);
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

            var result = await _context.Table.GetEntityIfExistsAsync<TableEntity>(partitionKey, rowKey).ConfigureAwait(false);

            if (result.HasValue)
                return await CreateSagaConsumeContext(new TableEntity(result.Value), SagaConsumeContextMode.Load).ConfigureAwait(false);

            return default;
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            var (insert, _) = TableInsert(context.Saga);
            return insert;
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            try
            {
                var eTag = context.GetPayload<SagaETag>();
                var dict = _context.Converter.GetDictionary(instance);
                TableEntity entity = new TableEntity(dict);
                entity.ETag  = new ETag(eTag.ETag.ToString());
                (entity.PartitionKey, entity.RowKey) = _context.Formatter.Format(instance.CorrelationId);

                await _context.Table.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace, context.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new SagaException("Saga update failed", typeof(TSaga), instance.CorrelationId, exception);
            }
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;

            var (partitionKey, rowKey) = _context.Formatter.Format(instance.CorrelationId);
            var eTag = context.GetPayload<SagaETag>();
            await _context.Table.DeleteEntityAsync(partitionKey, rowKey, new ETag(eTag.ETag.ToString()), context.CancellationToken).ConfigureAwait(false);
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

        (Task<Azure.Response>, TableEntity) TableInsert(TSaga instance)
        {
            var dict = _context.Converter.GetDictionary(instance);
            TableEntity entity = new TableEntity(dict);
            (entity.PartitionKey, entity.RowKey) = _context.Formatter.Format(instance.CorrelationId);

            return (_context.Table.AddEntityAsync(entity, CancellationToken), entity);
        }

        async Task<SagaConsumeContext<TSaga, TMessage>> CreateSagaConsumeContext(TableEntity entity, SagaConsumeContextMode mode)
        {
            var instance = _context.Converter.GetObject(entity);

            SagaConsumeContext<TSaga, TMessage> sagaConsumeContext = await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, mode)
                .ConfigureAwait(false);

            var eTag = new SagaETag(entity.ETag.ToString());

            sagaConsumeContext.AddOrUpdatePayload(() => eTag, _ => eTag);

            return sagaConsumeContext;
        }
    }


    public class CosmosTableSagaRepositoryContext<TSaga> :
        BasePipeContext,
        LoadSagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DatabaseContext<TSaga> _context;

        public CosmosTableSagaRepositoryContext(DatabaseContext<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            var (partitionKey, rowKey) = _context.Formatter.Format(correlationId);

            var result = await _context.Table.GetEntityAsync<TableEntity>(partitionKey, rowKey, cancellationToken: CancellationToken).ConfigureAwait(false);
            if (result.HasValue)
                return _context.Converter.GetObject(new TableEntity(result.Value));


            return default;
        }
    }
}
