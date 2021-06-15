namespace MassTransit.Azure.Table.Saga
{
    using System;
    using MassTransit.Saga;
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
