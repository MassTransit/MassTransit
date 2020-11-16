namespace MassTransit.Azure.Table.Saga
{
    using System;
    using MassTransit.Saga;


    public class ConstPartitionSagaKeyFormatter<TSaga> :
        ISagaKeyFormatter<TSaga>
        where TSaga : class, ISaga
    {
        readonly string _partitionKey;

        public ConstPartitionSagaKeyFormatter(string partitionKey)
        {
            _partitionKey = partitionKey;
        }

        public (string partitionKey, string rowKey) Format(Guid correlationId)
        {
            return (_partitionKey, correlationId.ToString());
        }
    }
}
