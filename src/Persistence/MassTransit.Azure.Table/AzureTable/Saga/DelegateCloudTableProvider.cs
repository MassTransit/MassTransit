namespace MassTransit.AzureTable.Saga
{
    using System;
    using Microsoft.Azure.Cosmos.Table;


    public class DelegateCloudTableProvider<TSaga> :
        ICloudTableProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<CloudTable> _cloudTable;

        public DelegateCloudTableProvider(Func<CloudTable> cloudTable)
        {
            _cloudTable = cloudTable;
        }

        public CloudTable GetCloudTable()
        {
            return _cloudTable();
        }
    }
}
