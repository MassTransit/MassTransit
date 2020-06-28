namespace MassTransit.Azure.Table.Saga
{
    using System;
    using MassTransit.Saga;


    public interface ISagaKeyFormatter<in TSaga>
        where TSaga : class, ISaga
    {
        (string partitionKey, string rowKey) Format(Guid correlationId);
    }
}
