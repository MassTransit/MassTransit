namespace MassTransit.Azure.Cosmos.Saga.Context
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;


    public class CosmosDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {

        public CosmosDatabaseContext(
            CosmosClient client,
            string databaseId,
            string containerId,
            Action<ItemRequestOptions> itemRequestOptions = null,
            Action<QueryRequestOptions> queryRequestOptions = null,
            Func<TSaga, PartitionKey> partitionKeyExpression = null)
        {
            Client = client;

            ItemRequestOptions = itemRequestOptions;

            QueryRequestOptions = queryRequestOptions;

            Container = client.GetContainer(databaseId, containerId);

            PartitionKeyExpression = partitionKeyExpression;
        }

        public CosmosClient Client { get; }
        public Container Container { get; }
        public Action<ItemRequestOptions> ItemRequestOptions { get; }
        public Action<QueryRequestOptions> QueryRequestOptions { get; }
        public Func<TSaga, PartitionKey> PartitionKeyExpression { get; }

        public async Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken)
        {
            try
            {
                ItemRequestOptions requestOptions = null;
                if(ItemRequestOptions != null)
                {
                    requestOptions = new ItemRequestOptions();
                    ItemRequestOptions(requestOptions);
                }

                var response = await Container.ReadItemAsync<TSaga>(correlationId.ToString(), new PartitionKey(correlationId.ToString()), requestOptions, cancellationToken).ConfigureAwait(false);

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
                var requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                ItemRequestOptions?.Invoke(requestOptions);

                await Container.CreateItemAsync(instance, PartitionKeyExpression?.Invoke(instance) ?? null, requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException)
            {
                throw new CosmosConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }

        public async Task<TSaga> Insert(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = true
                };

                ItemRequestOptions?.Invoke(requestOptions);

                var response = await Container.CreateItemAsync(instance, PartitionKeyExpression?.Invoke(instance) ?? null, requestOptions, cancellationToken).ConfigureAwait(false);

                return response.Resource;
            }
            catch (CosmosException)
            {
                throw new CosmosConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }

        public async Task Update(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                    IfMatchEtag = instance.ETag,
                };

                ItemRequestOptions?.Invoke(requestOptions);

                await Container.ReplaceItemAsync(instance, instance.CorrelationId.ToString(), PartitionKeyExpression?.Invoke(instance) ?? null, requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new CosmosConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }

        public async Task Delete(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                    IfMatchEtag = instance.ETag,
                };

                ItemRequestOptions?.Invoke(requestOptions);

                await Container.DeleteItemAsync<TSaga>(instance.CorrelationId.ToString(), PartitionKeyExpression?.Invoke(instance) ?? new PartitionKey(instance.CorrelationId.ToString()), requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new CosmosConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }
    }
}
