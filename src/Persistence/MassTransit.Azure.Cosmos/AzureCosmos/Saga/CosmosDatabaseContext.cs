namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using Microsoft.Azure.Cosmos;


    public class CosmosDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly Action<ItemRequestOptions> _itemRequestOptions;
        readonly Action<CosmosLinqSerializerOptions> _linqSerializerOptions;

        public CosmosDatabaseContext(Container container,
            Action<QueryRequestOptions> queryRequestOptions = null,
            Action<ItemRequestOptions> itemRequestOptions = null,
            Action<CosmosLinqSerializerOptions> linqSerializerOptions = null)
        {
            _itemRequestOptions = itemRequestOptions;
            _linqSerializerOptions = linqSerializerOptions;

            QueryRequestOptions = queryRequestOptions;

            Container = container;
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

        public CosmosLinqSerializerOptions GetLinqSerializerOptions()
        {
            if (_linqSerializerOptions == null)
                return default;

            var options = new CosmosLinqSerializerOptions();
            _linqSerializerOptions?.Invoke(options);

            return options;
        }
    }
}
