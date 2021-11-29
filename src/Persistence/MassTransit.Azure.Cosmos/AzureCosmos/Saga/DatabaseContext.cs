namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using Microsoft.Azure.Cosmos;


    public interface DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        Container Container { get; }
        Action<QueryRequestOptions> QueryRequestOptions { get; }

        ItemRequestOptions GetItemRequestOptions();
    }
}
