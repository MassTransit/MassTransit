# Dapper

[MassTransit.Dapper](https://www.nuget.org/packages/MassTransit.Dapper)

[Dapper][1] is a [super lightweight Micro-ORM][2] usable for saga persistence with Microsoft SQL Server. Dapper.Contrib is used for inserts and updates. The methods are virtual, so if you'd rather write the SQL yourself it is supported.

If you do not write your own sql, the model requires you use the `ExplicitKey` attribute for the `CorrelationId`. And if you have properties that are not available as columns, you can use the `Computed` attribute to not include them in the generated SQL. If you are using event correlation using other properties, it's highly recommended that you create indices for performance.

```csharp
public class OrderState :
    SagaStateMachineInstance
{
    [ExplicitKey]
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

### Container Integration

To configure Dapper as the saga repository for a saga, use the code shown below using the _AddMassTransit_ container extension.

```cs {4}
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository(connectionString);
});
```

The container extension will register the saga repository in the container. For more details on container configuration, review the [container configuration](/usage/containers/) section of the documentation.

### Limitations

#### Table Names

The tablename can only be the pluralized form of the class name. So `OrderState` would translate to table OrderState**s**. This applies even if you write your own SQL for updates and inserts.

#### Correlation Expressions

The expressions you can use for correlation is somewhat limited. These types of expressions are handled:

```cs
    x => x.CorrelationId == someGuid;
    x => x.IsDone;
    x => x.CorrelationId == someGuid && x.IsDone;
```

You can use multiple `&&` in the expression. What you can not use is `||` and negations. So a bool used like this `x.IsDone` can only be handled as true and nothing else.

[1]: https://dapper-tutorial.net/
[2]: https://github.com/StackExchange/Dapper
