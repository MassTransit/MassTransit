namespace MassTransit.AzureTable
{
    using System;


    public interface ISagaKeyFormatter<in TSaga>
        where TSaga : class, ISaga
    {
        (string partitionKey, string rowKey) Format(Guid correlationId);
    }
}
