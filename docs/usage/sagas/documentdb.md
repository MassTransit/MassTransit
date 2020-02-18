# DocumentDb

DocumentDb is the predecessor of Azure CosmosDb and the DocumentDb API is still one of the main APIs for the NoSQL document-oriented persistence of CosmosDb. MassTransit supports saga persistence in CosmosDb by using both MongoDb API (using the `MassTransit.MongoDb` package) and using DocumentDb API (using the `MassTransit.DocumentDb` package).

Out of the box, the only additional saga property required to use DocumentDb is _ETag_, which is managed by DocumentDb for optimistic concurrency. Once added to your saga class, when using the container configuration method below, the class is properly configured to map the _CorrelationId_ and _ETag_ properties to the associated `id` and `_etag` properties.

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

To configure DocumentDb as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension.

```cs {4-10}
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DocumentDbRepository(endpointUri, key, r =>
        {
            r.DatabaseId = "production-db"; // required

            // the default collection name, 'sagas', is used if not specified
            r.CollectionId = "sagas"; 
        });
});
```

To use the CosmosDb emulator, specify it in the configuration.

```cs {4-9}
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DocumentDbRepository(r =>
        {
            r.ConfigureEmulator();

            r.DatabaseId = "test";
        });
});
```

The container extension will register the saga repository in the container. For more details on container configuration, review the [container configuration](/usage/containers/) section of the documentation.

### Other Considerations

DocumentDb requires that any document stored there has a property called `id`, to be used as the document identity. Saga instances have `CorrelationId` for the same purpose, so there are two ways to create your DocumentDb saga class, which can have different implications depending on your usage. ETag must also be present, which is used for optimistic concurrency. Please never set this property yourself, it managed 100% by DocumentDb.

If event correlation expressions are used which include the _CorrelationId_ property, it's important to add the JSON property names that match what's used in DocumentDb.

```cs {5,11}
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

Another option is to inherit from the DocumentDb `Resource` class. In this example, the `id` property is overridden. 

```cs {6}
public class OrderState :
    Resource,
    SagaStateMachineInstance,
    IVersionedSaga
{
    [JsonProperty("id")]
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

