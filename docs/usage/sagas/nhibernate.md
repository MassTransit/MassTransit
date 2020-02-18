# NHibernate

> Package: [MassTransit.NHibernate](https://www.nuget.org/packages/MassTransit.NHibernate)

NHibernate is a widely used ORM and it is supported by MassTransit for saga storage. The example below shows the code-first approach to using NHibernate for saga persistence.

```cs
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }

    // If using Optimistic concurrency, this property is required
    public int Version { get; set; }
}
```

The instance properties are configured using a _SagaClassMapping_. 

::: warning Important
The `SagaClassMapping` has a default mapping for the `CorrelationId` as the primary key. If you create your own mapping, you must follow the same convention, or at least make it a Clustered Index + Unique, otherwise you will likely experience deadlock exceptions and/or performance issues in high throughput scenarios.
:::

```cs
public class OrderStateMap : 
    SagaClassMapping<OrderState>
{
    public OrderStateMap()
    {
        Property(x => x.CurrentState, x => x.Length(64));
        Property(x => x.OrderDate);

        Property(x => x.Version); // If using Optimistic concurrency
    }
}
```

### Container Integration

To configure NHibernate as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension. This will configure NHibernate to connect to the local NHibernate instance on the default port using Optimistic concurrency.

```cs {2,7}
// the session factory should be registered as a single instance
container.RegisterSingleInstance<ISessionFactory>(...);

container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .NHibernateRepository();
});
```

### Concurrency

NHibernate natively supports multiple concurrency handling mechanisms. The easiest is probably adding a `Version` property of type `int` to the saga instance class and map it to the column with the same name


