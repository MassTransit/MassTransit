namespace MassTransit.Azure.Table.Saga
{
    using System;


    public interface ISagaKeyFormatter<in TSaga>
        where TSaga : class, IVersionedSaga
    {
        (string partitionKey, string rowKey) Format(Guid correlationId);
    }
}
