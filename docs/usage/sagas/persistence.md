# Saga Persistence

Sagas are stateful event-based message consumers -- they retain state. Therefore, saving state between events is important. Without persistent state, a saga would consider each event a new event, and orchestration of subsequent events would be meaningless.

In order to store the saga state, you need to use one form of saga persistence. There are several types of storage that MassTransit supports, all of those, which are included to the main distribution, are listed below. There is also a in-memory unreliable storage, which allows to temporarily store your saga state. It is useful to try things out since it does not require any infrastructure.

### Order State

An example state machine instance is shown below. This example will be used across every storage engine to show how each is configured.

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

When using the _AddMassTransit_ container extension, the repository should be specified at saga registration. The example below specifies the InMemory saga repository.

```cs {4}
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .InMemoryRepository();
});
```

The saga repository is always registered with a singleton container lifecycle.

If the container registration is not being used, the InMemory saga repository can be created manually and specified on receive endpoint.

```cs
var orderStateMachine = new OrderStateMachine();
var repository = new InMemorySagaRepository<OrderState>();

var busControl = Bus.Factory.CreateUsingInMemory(x =>
{
    x.ReceiveEndpoint("order-state", e =>
    {
        e.StateMachineSaga(orderStateMachine, repository);
    });
});
```

There are two types of saga repository:
* Query repository
* Identity-only repository

Depending on the persistence mechanism, repository implementation can be either identity-only or identity plus query.

When using identity-only repository, such as Azure Service Bus message session or Redis, you can only use correlation by identity. This means that all events that the saga receives, must hold the saga correlation id, and the correlation for each event can only use `CorrelateById` method to define the correlation.

Query repository by definition support identity correlation too, but in addition support other properties of events being received and saga state properties. Such correlations are defined using `CorrelateBy` method and you can use any logical expression that involve the event data and saga state data to establish such correlation. Repository implementation such as Entity Framework, NHibernate and Marten support correlation by query. Of course, in-memory repository supports it as well.

### Identity

Saga instances are identified by a unique identifier (`Guid`), represented by the `CorrelationId` on the saga instance. Events are correlated to the saga instance using either the unique identifier, or alternatively using an expression that correlates properties on the saga instance to each event. If the `CorrelationId` is used, it's always a one-to-one match, either the saga already exists, or it's a new saga instance. With a correlation expression, the expression might match to more than one saga instance, so care should be used -- because the event would be delivered to all matching instances.

> Seriously, don't sent an event to all instances -- unless you want to watch your messages consumers lock your entire saga storage engine.

It is strongly advised to have `CorrelationId` as your table/document key. This will enable better concurrency handling and will make the saga state consistent.

### Publishing and Sending From Sagas

Sagas are completely message-driven and therefore not only consume but also publish events and send commands. However, if your saga received a lot of messages coming roughly at the same time and the endpoint is set to process multiple messages in parallel - this can lead to a conflict between message processing and saga persistence.

This means that there could be more than one saga state updates that are being persisted at the same time. Depending on the saga repository type, this might fail for different reasons - versioning issue, row or table lock or eTag mismatch. All those problems are basically saying that you are having a concurrency issue.

It is normal for the saga repository to throw an exception in such case but if your saga is publishing messages, they were already published but the saga state has not been updated. MassTransit will eventually use retry policy on the endpoint and more messages will be send, potentially leading to mess. Or, if there are no retry policies configured, messages might be sent indicating that the process needs to continue but saga instance will be in the old state and will not accept any further messages because they will come in a wrong state.

This issue is common and can be solved by postponing the message publish and send operations until all persistence work is done. All messages that should be published, are collected in a buffer, which is called *Outbox*. MassTransit implements this feature and it can be configured by adding these lines to your endpoint configuration:

```csharp
c.ReceiveEndpoint("queue", e =>
{
    e.UseInMemoryOutbox();
    // other endpoint configuration here
}
```

### Relational DB Recommendations

While it's nice if you are developing a green-field system and you can define your Saga Db Entity with CorrelationId as the Primary Key (Clustered), sometimes we have to work within existing db entities. If this is the case, please remember in order to keep your saga's performing quickly (optimistic OR pessimistic, it doesn't matter), follow the note below.

::: tip
The CorrelationId should preferably be the Primary Key + Clustered for your saga table. If unable, then it must be a Clustered Index + Unique. And it's also highly recommended to use the NewId package for creating nice Db Friendly guids.
:::

### Optimistic vs pessimistic concurrency

Most persistence mechanisms for sagas supported by MassTransit need some way to guarantee ACID when processing sagas. Because there can be multiple threads _consuming_ multiple bus events meant for the same saga instance, they could end up overwriting each other (race condition). 

Relational databases can easily handle this by setting the transaction type to *serializable* or (page/row) locking. This would be considered as _pessimistic concurrency_.

Another way to handle concurrency is to have some attribute like version or timestamp, which updates every time a saga is persisted. By doing that we can instruct the database only to update the record if this attribute matches between what we are trying to persist and what is stored in the database record we are trying to update.

This is type of concurrency is called an _optimistic concurrency_. It doesn't guarantee your unit of work with the database will succeed (must retry after these exceptions), but it also doesn't block anybody else from working within the same database page (not locking the table/page).

#### So, which one should I use?

For almost every scenario, it is recommended using the optimistic concurrency, because most state machine logic should be fairly quick.

If the chosen persistence method supports optimistic concurrency, race conditions can be handled rather easily by specifying a retry policy for concurrency exceptions or using generic retry policy.


[1]: https://www.postgresql.org/docs/9.5/static/functions-json.html
[3]: https://github.com/StackExchange/Dapper
[4]: https://github.com/StackExchange/Dapper/issues/889
