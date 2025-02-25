namespace MassTransit.AzureTable.Saga
{
    using System;
    using Azure.Data.Tables;


    public class DelegateCloudTableProvider<TSaga> :
        ICloudTableProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<TableClient> _cloudTable;

        public DelegateCloudTableProvider(Func<TableClient> cloudTable)
        {
            _cloudTable = cloudTable;
        }

        public TableClient GetCloudTable()
        {
            return _cloudTable();
        }
    }
}
