# Azure Table

Azure Tables are exposed in two ways in Azure - via Storage accounts & via the premium offering within Cosmos DB APIs. This persistence supports both implementations and behind the curtains uses the Microsoft.Azure.Cosmos.Table library for communication.

Out of the box, the only additional saga property required to use Azure Tables is _ETag_, which is managed by Azure for optimistic concurrency. Once added to your saga class, when using the container configuration method below, the class is properly configured to map the _CorrelationId_ and _ETag_ properties to the associated `id` and `_etag` properties.

::: tip NOTE
Azure Tables currently only supports Optimistic Concurrency.
:::

::: warning
Be sure to set DateTime properties as nullable when updated later in the saga. Failure to do this can result in 400 bad requests from Table Storage.
:::

```cs {10}
public class OrderState :
    SagaStateMachineInstance,
    IVersionedSaga
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }

    public string ETag { get; set; }
}
```

### Container Integration

To configure Cosmos DB as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension.

```cs
CloudTable cloudTable;
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .AzureTableRepository(endpointUri, key, r =>
        {
           cfg.ConnectionFactory(() => cloudTable);
        });
});
```

The container extension will register the saga repository in the container. For more details on container configuration, review the [container configuration](/usage/containers/) section of the documentation.