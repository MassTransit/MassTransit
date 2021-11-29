namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos;
    using Middleware;


    public class CosmosSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly DatabaseContext<TSaga> _context;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public CosmosSagaRepositoryContext(DatabaseContext<TSaga> context, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
            : base(consumeContext)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_context, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                var options = _context.GetItemRequestOptions() ?? new ItemRequestOptions();
                options.EnableContentResponseOnWrite = true;

                var partitionKey = new PartitionKey(instance.CorrelationId.ToString());

                ItemResponse<TSaga> response = await _context.Container.CreateItemAsync(instance, partitionKey, options, CancellationToken)
                    .ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await CreateSagaConsumeContext(response, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(exception, instance.CorrelationId);
            }

            return default;
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            try
            {
                var options = _context.GetItemRequestOptions();

                var id = correlationId.ToString();
                var partitionKey = new PartitionKey(id);

                ItemResponse<TSaga> response = await _context.Container.ReadItemAsync<TSaga>(id, partitionKey, options, CancellationToken)
                    .ConfigureAwait(false);

                return await CreateSagaConsumeContext(response, SagaConsumeContextMode.Load).ConfigureAwait(false);
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task Save(SagaConsumeContext<TSaga> context)
        {
            try
            {
                var options = _context.GetItemRequestOptions() ?? new ItemRequestOptions();
                options.EnableContentResponseOnWrite = false;

                var partitionKey = new PartitionKey(context.Saga.CorrelationId.ToString());

                await _context.Container.CreateItemAsync(context.Saga, partitionKey, options, context.CancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException exception)
            {
                throw new CosmosConcurrencyException("Saga Create Faulted", typeof(TSaga), context.Saga.CorrelationId, exception);
            }
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            try
            {
                var options = GetItemRequestOptions(context);

                var id = context.Saga.CorrelationId.ToString();
                var partitionKey = new PartitionKey(id);

                await _context.Container.ReplaceItemAsync(context.Saga, id, partitionKey, options, context.CancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new CosmosConcurrencyException("Saga Create Faulted", typeof(TSaga), context.Saga.CorrelationId, exception);
            }
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            try
            {
                var options = GetItemRequestOptions(context);

                var id = context.Saga.CorrelationId.ToString();
                var partitionKey = new PartitionKey(id);

                await _context.Container.DeleteItemAsync<TSaga>(id, partitionKey, options, context.CancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new CosmosConcurrencyException("Saga Create Faulted", typeof(TSaga), context.Saga.CorrelationId, exception);
            }
        }

        ItemRequestOptions GetItemRequestOptions(PipeContext context, bool enableContentResponseOnWrite = false)
        {
            var eTag = context.GetPayload<SagaETag>();

            var options = _context.GetItemRequestOptions() ?? new ItemRequestOptions();
            options.EnableContentResponseOnWrite = enableContentResponseOnWrite;
            options.IfMatchEtag = eTag.ETag;

            return options;
        }

        async Task<SagaConsumeContext<TSaga, TMessage>> CreateSagaConsumeContext(Response<TSaga> response, SagaConsumeContextMode mode)
        {
            var instance = response.Resource;

            SagaConsumeContext<TSaga, TMessage> sagaConsumeContext = await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, mode)
                .ConfigureAwait(false);

            var eTag = new SagaETag(response.ETag);

            sagaConsumeContext.AddOrUpdatePayload(() => eTag, _ => eTag);

            return sagaConsumeContext;
        }
    }


    public class CosmosSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DatabaseContext<TSaga> _context;

        public CosmosSagaRepositoryContext(DatabaseContext<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default)
        {
            QueryRequestOptions queryOptions = null;

            if (_context.QueryRequestOptions != null)
            {
                queryOptions = new QueryRequestOptions();
                _context.QueryRequestOptions?.Invoke(queryOptions);
            }

            // This will not work for Document Db because the .Where needs to look for [JsonProperty("id")],
            // and if you pass in CorrelationId property, the ISaga doesn't have that property. Can we .Select() it out?
            IEnumerable<TSaga> instances = await _context.Container.GetItemLinqQueryable<TSaga>(requestOptions: queryOptions)
                .Where(query.FilterExpression)
                .QueryAsync(cancellationToken)
                .ConfigureAwait(false);

            return new LoadedSagaRepositoryQueryContext<TSaga>(this, instances);
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            try
            {
                var options = _context.GetItemRequestOptions();

                var id = correlationId.ToString();
                var partitionKey = new PartitionKey(id);

                ItemResponse<TSaga> response = await _context.Container.ReadItemAsync<TSaga>(id, partitionKey, options, CancellationToken)
                    .ConfigureAwait(false);

                return response.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
        }
    }
}
