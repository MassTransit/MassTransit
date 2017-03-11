# Persisting Saga Instances

Sagas are stateful event-based message consumers -- they retain state. Therefore, saving state between 
events is important. Without persistent state, a saga would consider each event a new event, and orchestration 
of subsequent events would be meaningless.

<!-- TOC depthFrom:2 -->

- [Specifying saga persistence](#specifying-saga-persistence)
- [Identity](#identity)
- [Publishing and Sending From Sagas](#publishing-and-sending-from-sagas)
- [Storage Engines](#storage-engines)
    - [Entity Framework](#entity-framework)
    - [MongoDB](#mongodb)
    - [NHibernate](#nhibernate)
    - [Redis](#redis)
    - [Marten](#marten)
    - [Azure Service Bus](#azure-service-bus)

<!-- /TOC -->

## Specifying saga persistence

In order to store the saga state, you need to use one form of saga persistence. There are several
types of storage that MassTransit supports, all of those, which are included to the main distribution,
are listed below. There is also a in-memory unreliable storage, which allows to temporarily store
your saga state. It is useful to try things out since it does not require any infrastructure.

Simple initialization of a state machine saga with persistence looks like this:

```csharp
var sagaStateMachine = new ShoppingCartStateMachine();
var repository = new InMemorySagaRepository<ShoppingCart>();
var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
{
    var host = x.Host(new Uri("rabbitmq://localhost"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    x.ReceiveEndpoint(host, "shopping_cart_state", e =>
    {
        e.StateMachineSaga(sagaStateMachine, repository);
    });
});
```

It is important to notice that the saga repository object is a singleton. It does not hold any state
inside the class instance and only performs operations on the saga state objects that are send to it
to persist and retrieve.

There are two types of saga repository:
* Query repository
* Identity-only repository

Depending on the persistence mechanism, repository implementation can be either identity-only or
identity plus query.

When using identity-only repository, such as Azure Service Bus message session or Redis, you can
only use correlation by identity. This means that all events that the saga receives, must hold
the saga correlation id, and the correlation for each event can only use `CorrelateById` method 
to define the correlation.

Query repository by definition support identity correlation too, but in addition support other
properties of events being received and saga state properties. Such correlations are defined using
`CorrelateBy` method and you can use any logical expression that involve the event data and
saga state data to establish such correlation. Repository implementation such as Entity Framework,
NHibernate and Marten support correlation by query. Of course, in-memory repository supports it as well.

## Identity

Saga instances are identified by a unique identifier (`Guid`), represented by the `CorrelationId` on the saga instance. 
Events are correlated to the saga instance using either the unique identifier, or alternatively using an expression 
that correlates properties on the saga instance to each event. If the `CorrelationId` is used, it's always a 
one-to-one match, either the saga already exists, or it's a new saga instance. With a correlation expression, 
the expression might match to more than one saga instance, so care should be used -- because the event would be 
delivered to all matching instances.

> Seriously, don't sent an event to all instances -- unless you want to watch your messages consumers lock 
> your entire saga storage engine.

It is strongly advised to have `CorrelationId` as your table/document key. This will enable better
concurrency handling and will make the saga state consistent.

## Publishing and Sending From Sagas

Sagas are completely message-driven and therefore not only consume but also publish events and send commands. 
However, if your saga received a lot of messages coming roughly at the same time and the endpoint is set to 
process multiple messages in parallel - this can lead to a conflict between message processing and saga persistence.

This means that there could be more than one saga state updates that are being persisted at the same time. 
Depending on the saga repository type, this might fail for different reasons - versioning issue, row or table 
lock or eTag mismatch. All those problems are basically saying that you are having a concurrency issue.

It is normal for the saga repository to throw an exception in such case but if your saga is publishing messages, 
they were already published but the saga state has not been updated. MassTransit will eventually use retry policy 
on the endpoint and more messages will be send, potentially leading to mess. Or, if there are no retry policies 
configured, messages might be sent indicating that the process needs to continue but saga instance will be in the 
old state and will not accept any further messages because they will come in a wrong state.

This issue is common and can be solved by postponing the message publish and send operations until all 
persistence work is done. All messages that should be published, are collected in a buffer, which is 
called *Outbox*. MassTransit implements this feature and it can be configured by adding these lines to your 
endpoint configuration:

```csharp
c.ReceiveEndpoint("queue", e =>
{
    e.UseInMemoryOutbox();
    // other endpoint configuration here
}
```

## Storage Engines

MassTransit supports several storage engines, including NHibernate, Entity Framework, MongoDB and Redis. 
Each of these are setup in a similar way, but examples are shown below for each engine.

* [Entity Framework](#entity-framework)
* [NHibernate](#nhibernate)
* [MondoDB](#mongodb)
* [Redis](#redis)
* [Marten](#marten)
* [Azure Service Bus](#azure-service-bus)

### Entity Framework

Entity Framework seems to be the most common ORM for class-SQL mappings, and SQL is still widely used 
for storing data. So it's a win to have it supported out of the box by MassTransit.

#### Optimistic vs Pessimistic Concurrency
Most Relational Databases baked into MassTransit need some way to guarantee ACID when processing Sagas. Because there can be multiple threads _consuming_ multiple bus events meant for the same Saga Instance, they could end up overwriting eachother (Race Condition). Fortunately Relational DB's can easily handle this by setting the transaction type to Serializable or (Page/Row) locking. This would be considered __pessimistic concurrency__.

Fortunately Entity Framework is a Repository pattern itself, and they have baked in a Column Type `[Timestamp]` (a.k.a. `IsRowVersion` with fluent mapping) which will check the value of the column when it started it's "unit of work" and if that value is the same when updating that row in the database, everybody is happy!. But if that column value is different, then it was updated elsewhere. So EF will throw an exception `DbUpdateConcurrencyException`. This is __optimistic concurrency_. It doesn't guarantee your unit of work with the database will succeed (must retry these exceptions), but it also doesn't block anybody else from working within the same database page (not locking the table/page).

##### So Which one should I use?

For almost every scenario, I've used optimistic, because most state machine logic should be fairly quick. So
for most scenarios, optimistic concurrency should be fine, but you can choose what's the most appropriate for you.

The code-first mapping example below shows the basics of getting started. The lines have been commented
where the additional optimistic concurrency column is needed.

```csharp
public class SagaInstance : SagaStateMachineInstance
{
    public SagaInstance(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    protected SagaInstance()
    {
    }

    public string CurrentState { get; set; }
    public string ServiceName { get; set; }
    public Guid CorrelationId { get; set; }
    public byte[] RowVersion { get; set; } // For Optimistic Concurrency
}

public class SagaInstanceMap : SagaClassMapping<SagaInstance>
{
    public SagaInstanceMap()
    {
        Property(x => x.CurrentState);
        Property(x => x.ServiceName, x => x.Length(40));
        Property(x => x.RowVersion).IsRowVersion(); // For Optimistic Concurrency
    }
}
```

> Important:
> The `SagaClassMapping` has default mapping for the `CorrelationId` as a database generated primary key.
> If you use your own mapping, you must follow the same convention, otherwise there is a big chance to
> get deadlock exceptions in case of high throughput.

The repository is then created on the context factory for the `DbContext` is available.

```csharp
SagaDbContextFactory contextFactory = () => 
    new SagaDbContext<SagaInstance, SagaInstanceMap>(_connectionString);

var repository = new EntityFrameworkSagaRepository<SagaInstance>(contextFactory, optimistic: true); // true For Optimistic Concurrency, false is default for pessimistic
```

Lastly, this snippit below is __only needed for optimistic concurrency__, because the saga should retry processing
if a failure occurred when writing to the database. This snippit is adding a [retry policies](retries.md) middleware to the
saga receive endpoint from the [first section](#specifying-saga-persistence).

```csharp
    ...
    x.ReceiveEndpoint(host, "shopping_cart_state", e =>
    {
        e.UseRetry(x => {
            x.Handle<DbUpdateConcurrencyException>();
            x.Interval(5, TimeSpan.FromMilliseconds(100)));
        }); // Add Retry Middleware for Optimistic Concurrency
        e.StateMachineSaga(sagaStateMachine, repository);
    });
    ...
```

### MongoDB

MongoDB is an easy to use saga repository, because setup is easy. There is no need for class mapping, 
the saga instances can be persisted easily using a MongoDB collection.

```csharp
public class SagaInstance : SagaStateMachineInstance
{
    public SagaInstance(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    protected SagaInstance() { }

    public string CurrentState { get; set; }
    public string ServiceName { get; set; }
    public Guid CorrelationId { get; set; }
}
```

The saga repository is created using the simple syntax:

```csharp
var database = new MongoClient("mongodb://127.0.0.1").GetDatabase("sagas");
var repository = new MongoDbSagaRepository<SagaInstance>(database);
```

Each saga instance will be placed in a collection specific to the instance type.

### NHibernate

Although NHibernate is not being actively developed recently, it is still widely used and 
is supported by MassTransit for saga storage. The example below shows the code-first approach 
to using NHibernate for saga persistence.

```csharp
public class SagaInstance : SagaStateMachineInstance
{
    public SagaInstance(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    protected SagaInstance() { }

    public string CurrentState { get; set; }
    public string ServiceName { get; set; }
    public Guid CorrelationId { get; set; }
}

public class SagaInstanceMap : SagaClassMapping<SagaInstance>
{
    public SagaInstanceMap()
    {
        Property(x => x.CurrentState);
        Property(x => x.ServiceName, x => x.Length(40));
    }
}
```

The `SagaClassMapping` base class maps the `CorrelationId` of the saga, and handles some of the basic 
bootstrapping of the class map. All other properties, including the property for the `CurrentState` 
(if you're using state machine sagas), must be mapped by the developer. Once mapped, the `ISessionFactory` 
can be created using NHibernate directly. From the session factory, the saga repository can be created.

> Important:
> The `SagaClassMapping` has default mapping for the `CorrelationId` as a database generated primary key.
> If you use your own mapping, you must follow the same convention, otherwise there is a big chance to
> get deadlock exceptions in case of high throughput.

```csharp
ISessionFactory sessionFactory = CreateSessionFactory();
var repository = new NHibernateSagaRepository<SagaInstance>(sessionFactory);
```

### Redis

Redis is a very popular key-value store, which is known for being very fast.

Redis does not support queries, therefore Redis saga persistence only supports correlation by id. 
If you try to use correlation by expressions, you will get a "not implemented" exception.

Saga persistence for Redis uses `ServiceStack.Redis` library and it support both BSD-licensed v3.9.71 
and the latest commercial versions as well.

Saga instance class must implement `IHasGuid` interface and the `Id` property, that must return the 
value of the `CorrelationId` property:

```csharp
public class SagaInstance : 
    SagaStateMachineInstance, IHasGuidId
{
    public Guid CorrelationId { get; set; }
    public Guid Id => CorrelationId;
    public string CurrentState { get; set; }

    public string CustomData { get; set; }
}
```

Redis saga persistence does not aquire locking on the database record when writing it so potentially 
you can have write conflict in case the saga is updating its state frequently (hundreds of times per second). 
To resolve this, the saga instance can implement the `IVersionedSaga` inteface and include the Version property:

```csharp
public int Version { get; set; }
```

When version of the instance that is being updated will be lower than the expected version, 
the saga repository will trow an exception and force the message to be retried, potentially resolving the issue.

The Redis saga repository requires `ServiceStack.Redis.IRedisClientsManager` as constructor parameter. 
For containerless initialization the code would look like:

```csharp
var redisConnectionString = "redis://localhost:6379";
var repository = new RedisSagaRepository<SagaInstance>(
    new RedisManagerPool(redisConnectionString));
```

If you use a container, you can use the code like this (example for Autofac):

```csharp
var redisConnectionString = "redis://localhost:6379";
builder.Register<IRedisClientsManager>(c => 
    new RedisManagerPool(redisConnectionString)).SingleInstance();
builder.RegisterGeneric(typeof(RedisSagaRepository<>))
    .As(typeof(ISagaRepository<>)).SingleInstance();
```

### Marten

[Marten][2] is an open-source library that provides an API to the PostgreSQL [JSONB storage][1], influenced by
RavenDb client API. It allows to use PotgreSQL as schema-less NoSQL document storage. Unlike typical document
databases, PostgreSQL JSONB storage provides you the ACID-compliant transactional store with full consistency.

To use Marten and PostgreSQL as saga persistence, you need to install `MassTransit.Marten` NuGet package and
add some code.

First, your saga state class needs to mark the correlationId property with the `[Identity]` arrtibute. By this
you inform Marten that correlationId will be used as the primary key.

```csharp
public class SampleSaga : ISaga
{
    [Identity]
    public Guid CorrelationId { get; set; }
    public string State { get; set; }
    public string SomeProperty { get; set; }
}
```

Then you need to initialize the document store and the repository. Repository needs the store as its constructor
parameter.

```csharp
var connectionString =
    "server=localhost;port=5432;database=test;user id=test;password=test;";
var store = DocumentStore.For(connectionString);
var repository = new MartenSagaRepository<SampleSaga>(store);
```

If you use a container, you can use the code like this (example for Autofac):

```csharp
var connectionString =
    "server=localhost;port=5432;database=test;user id=test;password=test;";
builder.Register<IDocumentStore>(c => DocumentStore.For(connectionString);
builder.RegisterGeneric(typeof(MartenSagaRepository<>))
    .As(typeof(ISagaRepository<>)).SingleInstance();
```

Marten will create necessary tables for you. This type of saga repository
supports correlation by id and custom expressions.

### Azure Service Bus

Azure Service Bus provides a feature called *message sessions*, to process multiple messages at once and 
to store some state on a temporary basis, which can be retrieved by some key.

The latter give us an ability to use this feature as saga state storage. Using message sessions
as saga persistence, you can only use Azure Service Bus for both messaging and saga persistencepurposes,
without needing any additional infrastructure.

There is a limitation for using message sessions - this feature is not supported for AMQP transport.

You have to explicitly enable message sessions when configuring the endpoint, and use parameterless
constructor to instantiate the saga repository.

Here is the basic sample of how to use the Azure Service Bus message session as saga repository:

```csharp
var sagaStateMachine = new MySagaStateMachine();
var repository = new MessageSessionSagaRepository<MySaga>(); 
sbc.ReceiveEndpoint(host, "test_queue", ep =>
{
    ep.RequiresSession = true;
    ep.StateMachineSaga(sagaStateMachine, repository);
});
```

As mentioned before, the message session allows storing and retrieving any state by some unique key.
This means that this type of saga persistence only support correlation by id. So, similar to Redis
saga persistence, you cannot use `CorralateBy` to specify how to find the saga instance, but only
`CorrelateById`.



[1]: https://www.postgresql.org/docs/9.5/static/functions-json.html
[2]: http://jasperfx.github.io/marten/