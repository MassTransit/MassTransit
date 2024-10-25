namespace MassTransit.AzureTable.Saga
{
    using Azure.Data.Tables;


    public class ConstCloudTableProvider<TSaga> :
        ICloudTableProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly TableClient _cloudTable;

        public ConstCloudTableProvider(TableClient cloudTable)
        {
            _cloudTable = cloudTable;
        }

        public TableClient GetCloudTable()
        {
            return _cloudTable;
        }
    }
}
