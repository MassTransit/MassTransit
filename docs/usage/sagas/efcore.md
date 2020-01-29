# Entity Framework Core

[MassTransit.EntityFrameworkCore](https://www.nuget.org/packages/MassTransit.EntityFrameworkCore)

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
        entity.Property(x => x.CurrentState).HasMaxLength(64);
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
    public OrderStateDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}
```

### Container Integration

Once the class map and associated _DbContext_ class have been created, the saga repository can be configured with the saga registration, which is done using the configuration method passed to _AddMassTransit_. The following example shows how the repository is configured for using Microsoft Dependency Injection Extensions, which are used by default with Entity Framework Core.

```cs
services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

            r.AddDbContext<DbContext, OrderStateDbContext>((provider,builder) =>
            {
                builder.UseSqlServer(connectionString, m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                });
            });
        });
});
```



