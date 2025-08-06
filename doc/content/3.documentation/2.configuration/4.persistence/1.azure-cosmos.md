# Azure Cosmos DB

[![alt NuGet](https://img.shields.io/nuget/v/MassTransit.Azure.Cosmos.svg "NuGet")](https://nuget.org/packages/MassTransit.Azure.Cosmos/)

When using Azure Cosmos DB, no additional saga properties are required. An Azure Cosmos DB document has an `_etag` used for optimistic concurrency, however, the saga instance class does not require it. MassTransit manages the *_etag* property under the hood using a *payload* on the `SagaConsumeContext`.

```csharp 
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

To obtain the saga instance *_etag* value:

```csharp
string eTag = context.TryGetPayload<SagaETag>(out var payload) ? payload.ETag : null;
```

## Configuration

To configure Cosmos DB as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension.

```csharp 
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .CosmosRepository(endpointUri, key, r =>
        {
            r.DatabaseId = "production-db"; // required

            // kebab case formatter is used by default if not specified (OrderState -> order-state)
            r.CollectionId = "sagas";
        });
});
```

To use the CosmosDb emulator, specify it in the configuration.

```csharp 
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .CosmosRepository(r =>
        {
            r.ConfigureEmulator();

            r.DatabaseId = "test";
        });
});
```

The container extension will register the saga repository in the container. For more details on container configuration, review the [configuration](/documentation/patterns/saga) section of the documentation.

## Other Considerations

When using Azure Cosmos, documents must have an `id` property which is used as the document's identity. MassTransit saga instance use a `CorrelationId` property to identify the instance. 

So there are two ways to create your Cosmos DB saga class, which can have different implications depending on your usage. ETag must also be present, which is used for optimistic concurrency. Please never set this property yourself, it managed 100% by Cosmos DB.

If event correlation expressions are used which include the _CorrelationId_ property, it's important to add the JSON property names that match what's used in Cosmos DB.

```csharp {5,11}
public class OrderState :
    SagaStateMachineInstance,
    IVersionedSaga
{
    [JsonProperty("id")]
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }

    [JsonProperty("_etag")]
    public string ETag { get; set; }
}
```
