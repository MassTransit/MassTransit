# DynamoDb

[![alt NuGet](https://img.shields.io/nuget/v/MassTransit.DynamoDb.svg "NuGet")](https://nuget.org/packages/MassTransit.DynamoDb/)

DynamoDB is a fully managed NoSQL database provided by AWS.
Setting it up as a SagaRepository is simple, but do note that it requires implementing the _ISagaVersion_ interface
that requires a _Version_ property for optimistic concurrency:

```csharp
public class OrderState :
    SagaStateMachineInstance,
    ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; }
    public DateTime? OrderDate { get; set; }
}
```

## Configuration
In Order to configure DynamoDB as a saga repository, use the *AddMassTransit* container extension:

```csharp
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DynamoDbRepository(config =>
        {
            // required
            config.TableName = "Orders";
             // required. Refer to AWS SDK docs for how to create a context.
            config.ContextFactory(provider => new DynamoDBContext(dynamoDbClient));
        });
});
```

The code above configures the repository to store saga instances in a table named `Orders`.
A `DynamoDBContext` must be created from an existing `AmazonDynamoDBClient` instance.
For example, if a local dynamodb instance is used, the code creating a client could look like this:

```csharp
var dynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig { ServiceURL = "http://localhost:4566" });
```


