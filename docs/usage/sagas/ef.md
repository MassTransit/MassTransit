# Entity Framework

> [MassTransit.EntityFramework](https://www.nuget.org/packages/MassTransit.EntityFramework)

Entity Framework is a commonly used ORM used with SQL.

An example saga instance is shown below, which is orchestrated using an Automatonymous state machine. The _CorrelationId_ will be the primary key, and _CurrentState_ will be used to store the current state of the saga instance. 

```cs
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }

    // If using Optimistic concurrency, this property is required
    public byte[] RowVersion { get; set; }
}
```

The instance properties are configured using a _SagaClassMap_. 

::: warning Important
The `SagaClassMap` has a default mapping for the `CorrelationId` as the primary key. If you create your own mapping, you must follow the same convention, or at least make it a Clustered Index + Unique, otherwise you will likely experience deadlock exceptions and/or performance issues in high throughput scenarios.
:::

```cs
public class OrderStateMap : 
    SagaClassMap<OrderState>
{
    protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).Length(64);
        entity.Property(x => x.OrderDate);

        // If using Optimistic concurrency, otherwise remove this property
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}
```

Include the instance map in a _DbContext_ class that will be used by the saga repository.

```cs
public class OrderStateDbContext : 
    SagaDbContext
{
    public OrderStateDbContext(string nameOrConnectionString) 
        : base(nameOrConnectionString)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}
```

### Container Integration

Once the class map and associated _DbContext_ class have been created, the saga repository can be configured with the saga registration, which is done using the configuration method passed to _AddMassTransit_. The following example shows how the repository is configured for using Microsoft Dependency Injection Extensions, which are used by default with Entity Framework.

> When using container configuration, the `DbContext` used by the saga repository is scoped.

```cs
services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

            r.AddDbContext<DbContext, OrderStateDbContext>(connectionString)
        });
});
```

### Optimistic Concurrency

If you are using optimistic concurrency, it's best to configure the endpoint to retry on concurrency exceptions.

```cs
services.AddMassTransit(x =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Optimistic;

            r.DatabaseFactory(() => new OrderStateDbContext(connectionString));
        });

    x.AddBus(context => Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ReceiveEndpoint("order-state", e =>
        {
            e.UseRetry(r => 
            {
                r.Handle<DbUpdateConcurrencyException>();

                // This is the SQLServer error code for duplicate key, if you are using another database,
                // the code might be different
                r.Handle<DbUpdateException>(y => y.InnerException is SqlException e && e.Number == 2627);

                r.Interval(5, TimeSpan.FromMilliseconds(100));
            });

            e.ConfigureSaga<OrderState>(context);
        });
    }));
});
```

> If you have retry policy without an exception filter, it will also handle the concurrency exception, so explicit configuration is not required in this case.

