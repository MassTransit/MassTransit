# Marten

> Package: [MassTransit.Marten](https://nuget.org/packages/MassTransit.Marten)

[Marten][2] is an open source library that provides provides .NET developers with the ability to easily use the proven PostgreSQL database engine and its fantastic [JSON support][1] as a fully fledged document database. To use Marten and PostgreSQL as saga persistence, you need to install `MassTransit.Marten` NuGet package and add some code.

> MassTransit will automatically configure the _CorrelationId_ property so that Marten will use that property as the primary key. No attribute is necessary.

```cs
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

### Container Integration

To configure Marten as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension. This will configure Marten to connect to the local Marten instance on the default port using Optimistic concurrency.

```cs {6}
container.AddMassTransit(cfg =>
{
    var connectionString = "server=localhost;port=5432;database=orders;user id=web;password=webpw;";

    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository(connectionString);
});
```

### Optimistic Concurrency

To use Marten's built-in Optimistic concurrency, use the configuration options to configure the schema. Marten supports optimistic concurrency by using an eTag-like version field in the metadata, which does not require any additional fields in the saga class.

```cs {8}
container.AddMassTransit(cfg =>
{
    var connectionString = "server=localhost;port=5432;database=orders;user id=web;password=webpw;";

    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository(connectionString, r =>
        {
            r.Schema.For<OrderState>().UseOptimisticConcurrency(true);
        });
});
```

Alternatively, you can add the `UseOptimisticConcurrency` attribute to the class.

```cs
[UseOptimisticConcurrency]
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    // ...
}
```

### Index Creation

Marten can create indices for properties, which greatly increases query performance. If your saga is correlating events using other fields, index creation is recommended. For example, if an _OrderNumber_ property was added to the _OrderState_ class, it could be indexed by configuring it in the repository.

```cs {7}
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public string OrderNumber { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

```cs {8}
container.AddMassTransit(cfg =>
{
    var connectionString = "server=localhost;port=5432;database=orders;user id=web;password=webpw;";

    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository(connectionString, r =>
        {
            r.Schema.For<OrderState>().Index(x => x.OrderNumber);
        });
});
```

Details on how Marten creates indices is available in the [Computed Index](https://martendb.io/documentation/documents/customizing/computed_index/) documentation.

[1]: https://www.postgresql.org/docs/9.5/static/functions-json.html
[2]: http://jasperfx.github.io/marten/
