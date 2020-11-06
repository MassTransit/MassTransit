# Azure Table

Azure Tables are exposed in two ways in Azure - via Storage accounts & via the premium offering within Cosmos DB APIs. This persistence supports both implementations and behind the curtains uses the Microsoft.Azure.Cosmos.Table library for communication.

::: tip NOTE
Azure Tables currently only supports Optimistic Concurrency. Mass Transit manages the ETag property in Payload Context and uses this property for state machine updates. Concurrency errors can be spotted in logs via standard "Precondition Failed" errors from Table Storage.
:::

::: warning
Be sure to set DateTime properties as nullable when updated later in the saga. Failure to do this can result in 400 bad requests from Table Storage.
:::

```cs {10}
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
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