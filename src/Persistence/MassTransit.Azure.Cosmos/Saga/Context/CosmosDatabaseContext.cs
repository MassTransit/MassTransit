namespace MassTransit.Azure.Cosmos.Saga.Context
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;


    public class CosmosDatabaseContext<TSaga> :
        DatabaseContext<TSaga>,
        IDisposable
        where TSaga : class, IVersionedSaga
    {
        readonly CosmosClient _client;

        public CosmosDatabaseContext(Container container, Action<ItemRequestOptions> itemRequestOptions = null,
            Action<QueryRequestOptions> queryRequestOptions = null)
        {
            ItemRequestOptions = itemRequestOptions;

            QueryRequestOptions = queryRequestOptions;

            Container = container;
        }

        internal CosmosDatabaseContext(CosmosClient client, Container container, Action<ItemRequestOptions> itemRequestOptions,
            Action<QueryRequestOptions> queryRequestOptions)
            : this(container, itemRequestOptions, queryRequestOptions)
        {
            _client = client;
        }

        public Container Container { get; }
        public Action<ItemRequestOptions> ItemRequestOptions { get; }
        public Action<QueryRequestOptions> QueryRequestOptions { get; }

        public async Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken)
        {
            try
            {
                ItemRequestOptions requestOptions = null;
                if (ItemRequestOptions != null)
                {
                    requestOptions = new ItemRequestOptions();
                    ItemRequestOptions(requestOptions);
                }

                var id = correlationId.ToString();
                var partitionKey = new PartitionKey(id);

                ItemResponse<TSaga> response = await Container.ReadItemAsync<TSaga>(id, partitionKey, requestOptions, cancellationToken).ConfigureAwait(false);

                return response.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task Add(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = false};
                ItemRequestOptions?.Invoke(requestOptions);

                var partitionKey = new PartitionKey(instance.CorrelationId.ToString());

                await Container.CreateItemAsync(instance, partitionKey, requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException exception)
            {
                throw new CosmosConcurrencyException(
                    $"Unable to add saga {instance.CorrelationId}. It may not have been found or may have been updated by another process.", exception);
            }
        }

        public async Task<TSaga> Insert(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = true};
                ItemRequestOptions?.Invoke(requestOptions);

                var partitionKey = new PartitionKey(instance.CorrelationId.ToString());

                ItemResponse<TSaga> response = await Container.CreateItemAsync(instance, partitionKey, requestOptions, cancellationToken).ConfigureAwait(false);

                return response.Resource;
            }
            catch (CosmosException exception)
            {
                throw new CosmosConcurrencyException(
                    $"Unable to insert saga {instance.CorrelationId}. It may not have been found or may have been updated by another process.", exception);
            }
        }

        public async Task Update(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                    IfMatchEtag = instance.ETag
                };

                ItemRequestOptions?.Invoke(requestOptions);

                var id = instance.CorrelationId.ToString();
                var partitionKey = new PartitionKey(id);

                await Container.ReplaceItemAsync(instance, id, partitionKey, requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new CosmosConcurrencyException(
                    $"Unable to update saga {instance.CorrelationId}. It may not have been found or may have been updated by another process.", exception);
            }
        }

        public async Task Delete(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                    IfMatchEtag = instance.ETag
                };

                ItemRequestOptions?.Invoke(requestOptions);

                var id = instance.CorrelationId.ToString();
                var partitionKey = new PartitionKey(id);

                await Container.DeleteItemAsync<TSaga>(id, partitionKey, requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new CosmosConcurrencyException(
                    $"Unable to delete saga {instance.CorrelationId}. It may not have been found or may have been updated by another process.", exception);
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
