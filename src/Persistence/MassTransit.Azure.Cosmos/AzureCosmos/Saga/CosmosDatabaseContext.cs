namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using Microsoft.Azure.Cosmos;


    public class CosmosDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly Action<ItemRequestOptions> _itemRequestOptions;

        public CosmosDatabaseContext(Container container, Action<ItemRequestOptions> itemRequestOptions = null,
            Action<QueryRequestOptions> queryRequestOptions = null)
        {
            _itemRequestOptions = itemRequestOptions;

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
    }
}
