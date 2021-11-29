namespace MassTransit.AzureTable.Saga
{
    using Microsoft.Azure.Cosmos.Table;


    public class ConstCloudTableProvider<TSaga> :
        ICloudTableProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly CloudTable _cloudTable;

        public ConstCloudTableProvider(CloudTable cloudTable)
        {
            _cloudTable = cloudTable;
        }

        public CloudTable GetCloudTable()
        {
            return _cloudTable;
        }
    }
}
