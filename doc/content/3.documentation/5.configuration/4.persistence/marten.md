# Marten

[![alt NuGet](https://img.shields.io/nuget/v/MassTransit.Marten.svg "NuGet")](https://nuget.org/packages/MassTransit.Marten/)

[Marten][2] is an open source library that provides provides .NET developers with the ability to easily use the proven PostgreSQL database engine and its fantastic [JSON support][1] as a fully fledged document database. To use Marten and PostgreSQL as saga persistence, you need to install `MassTransit.Marten` NuGet package and add some code.

> MassTransit will automatically configure the _CorrelationId_ property so that Marten will use that property as the primary key. No attribute is necessary.

```csharp
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

## Configuration

To configure Marten and use it as a saga repository with MassTransit, use the `MartenRepository` method when adding the saga.

```csharp
services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=orders;username=web;password=webpw;";
    
    options.Connection(connectionString);
});

services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository();
});
```

### Repository Provider

When adding sagas using any of the `AddSagas` or `AddSagaStateMachines` methods, the Marten saga repository provider can be used to automatically configure Marten as the saga repository. 

```csharp
services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=orders;username=web;password=webpw;";
    
    options.Connection(connectionString);
});

services.AddMassTransit(x =>
{
    x.SetMartenSagaRepositoryProvider();

    var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
    
    x.AddSagaStateMachines(entryAssembly);
});
```

To configure the saga repository for a specific saga type, use the `AddSagaRepository` method and specify the appropriate saga repository.

```csharp
services.AddMassTransit(x =>
{
    x.SetMartenSagaRepositoryProvider();

    var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
    
    x.AddSagaStateMachines(entryAssembly);
    
    x.AddSagaRepository<OrderState>()
        .MartenRepository();
});
```

## Optimistic Concurrency

To use Marten's built-in optimistic concurrency, which uses an *eTag*-like version metadata field, an additional schema configuration may be specified.

> This does **not** require any additional fields in the saga class.

```csharp
services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository(r => r.UseOptimisticConcurrency(true));
});
```

Alternatively, you can add the `UseOptimisticConcurrency` attribute to the saga class.

```csharp
[UseOptimisticConcurrency]
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    // ...
}
```

## Index Creation

Marten can create indices for properties, which greatly increases query performance. If your saga is correlating events using other saga properties, index creation is recommended. For example, if an _OrderNumber_ property was added to the _OrderState_ class, it could be indexed by configuring it in the repository.

```csharp
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public string OrderNumber { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

```csharp
services.AddMarten(options =>
{
    const string connectionString = "host=localhost;port=5432;database=orders;username=web;password=webpw;";
    
    options.Connection(connectionString);
});

services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository(r => r.Index(x => x.OrderNumber));
});
```

Details on how Marten creates indices is available in the [Indexing](https://martendb.io/documents/indexing/) documentation.

[1]: https://www.postgresql.org/docs/9.5/static/functions-json.html
[2]: http://jasperfx.github.io/marten/
