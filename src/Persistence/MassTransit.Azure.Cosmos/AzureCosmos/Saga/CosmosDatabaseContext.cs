namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using Microsoft.Azure.Cosmos;


    public class CosmosDatabaseContext<TSaga> :
        DatabaseContext<TSaga>,
        IDisposable
        where TSaga : class, ISaga
    {
        readonly CosmosClient _client;
        readonly Action<ItemRequestOptions> _itemRequestOptions;

        public CosmosDatabaseContext(Container container, Action<ItemRequestOptions> itemRequestOptions = null,
            Action<QueryRequestOptions> queryRequestOptions = null)
        {
            _itemRequestOptions = itemRequestOptions;

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

        public Action<QueryRequestOptions> QueryRequestOptions { get; }

        public ItemRequestOptions GetItemRequestOptions()
        {
            if (_itemRequestOptions == null)
                return default;

            var options = new ItemRequestOptions();
            _itemRequestOptions(options);

            return options;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
