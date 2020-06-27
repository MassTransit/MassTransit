namespace MassTransit.Azure.Table.Saga
{
    using System;


    public class ConstantSagaKeyFormatter<TSaga> :
        ISagaKeyFormatter<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly string _partitionKey;

        public ConstantSagaKeyFormatter(string partitionKey)
        {
            _partitionKey = partitionKey;
        }

        public (string partitionKey, string rowKey) Format(Guid correlationId)
        {
            return (_partitionKey, correlationId.ToString());
        }
    }
}
