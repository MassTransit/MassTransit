namespace MassTransit.Azure.Cosmos.Saga.Context
{
    using System;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos;


    public interface DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        Container Container { get; }
        Action<QueryRequestOptions> QueryRequestOptions { get; }

        ItemRequestOptions GetItemRequestOptions();
    }
}
