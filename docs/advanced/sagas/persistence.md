# Persisting Saga Instances

Sagas are stateful event-based message consumers -- they retain state. Therefore, saving state between events is important. Without persistent state, a saga would consider each event a new event, and orchestration of subsequent events would be meaningless.

- [Specifying saga persistence](#specifying-saga-persistence)
- [Identity](#identity)
- [Publishing and Sending From Sagas](#publishing-and-sending-from-sagas)
- [Storage Engines](#storage-engines)
    - [Entity Framework](#entity-framework)
    - [MongoDB](#mongodb)
    - [NHibernate](#nhibernate)
    - [Redis](#redis)
    - [Marten](#marten)
    - [DocumentDb](#documentdb)
    - [Azure Service Bus](#azure-service-bus)

## Specifying saga persistence

In order to store the saga state, you need to use one form of saga persistence. There are several types of storage that MassTransit supports, all of those, which are included to the main distribution, are listed below. There is also a in-memory unreliable storage, which allows to temporarily store your saga state. It is useful to try things out since it does not require any infrastructure.

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

It is important to notice that the saga repository object is a singleton. It does not hold any state inside the class instance and only performs operations on the saga state objects that are send to it to persist and retrieve.

There are two types of saga repository:
* Query repository
* Identity-only repository

Depending on the persistence mechanism, repository implementation can be either identity-only or identity plus query.

When using identity-only repository, such as Azure Service Bus message session or Redis, you can only use correlation by identity. This means that all events that the saga receives, must hold the saga correlation id, and the correlation for each event can only use `CorrelateById` method to define the correlation.

Query repository by definition support identity correlation too, but in addition support other properties of events being received and saga state properties. Such correlations are defined using `CorrelateBy` method and you can use any logical expression that involve the event data and saga state data to establish such correlation. Repository implementation such as Entity Framework, NHibernate and Marten support correlation by query. Of course, in-memory repository supports it as well.

## Identity

Saga instances are identified by a unique identifier (`Guid`), represented by the `CorrelationId` on the saga instance. Events are correlated to the saga instance using either the unique identifier, or alternatively using an expression that correlates properties on the saga instance to each event. If the `CorrelationId` is used, it's always a one-to-one match, either the saga already exists, or it's a new saga instance. With a correlation expression, the expression might match to more than one saga instance, so care should be used -- because the event would be delivered to all matching instances.

> Seriously, don't sent an event to all instances -- unless you want to watch your messages consumers lock your entire saga storage engine.

It is strongly advised to have `CorrelationId` as your table/document key. This will enable better concurrency handling and will make the saga state consistent.

## Publishing and Sending From Sagas

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

## Storage Engines

MassTransit supports several storage engines, including NHibernate, Entity Framework, MongoDB and Redis. Each of these are setup in a similar way, but examples are shown below for each engine.

* [Entity Framework](#entity-framework)
* [NHibernate](#nhibernate)
* [MongoDB](#mongodb)
* [Redis](#redis)
* [Marten](#marten)
* [DocumentDb](#documentdb)
* [Azure Service Bus](#azure-service-bus)
* [Dapper](#dapper)

### Optimistic vs pessimistic concurrency

Most persistence mechanisms for sagas supported by MassTransit need some way to guarantee ACID when processing sagas. Because there can be multiple threads _consuming_ multiple bus events meant for the same saga instance, they could end up overwriting each other (race condition). 

Relational databases can easily handle this by setting the transaction type to *serializable* or (page/row) locking. This would be considered as _pessimistic concurrency_.

Another way to handle concurrency is to have some attribute like version or timestamp, which updates every time a saga is persisted. By doing that we can instruct the database only to update the record if this attribute matches between what we are trying to persist and what is stored in the database record we are trying to update.

This is type of concurrency is called an _optimistic concurrency_. It doesn't guarantee your unit of work with the database will succeed (must retry after these exceptions), but it also doesn't block anybody else from working within the same database page (not locking the table/page).

#### So, which one should I use?

For almost every scenario, it is recommended using the optimistic concurrency, because most state machine logic should be fairly quick. 

If the chosen persistence method supports optimistic concurrency, race conditions can be handled rather easily by specifying a retry policy for concurrency exceptions or using generic retry policy.

### Entity Framework

Entity Framework seems to be the most common ORM for class-SQL mappings, and SQL is still widely used for storing data. So it's a win to have it supported out of the box by MassTransit.

#### Concurrency handling
Fortunately, Entity Framework is a repository pattern itself, and has a column type `[Timestamp]` (a.k.a. `IsRowVersion` with fluent mapping), which will check the value of the column when it starts it's "unit of work" and if that value is the same when updating that row in the database - everybody is happy! But, if that column value is different, then it has been updated elsewhere. So, the Entity Framework will throw a `DbUpdateConcurrencyException`, which can be handled by a retry policyto fix the concurrency violation.

#### Configuration and usage
The code-first mapping example below shows the basics of getting started. The lines have been commented where the additional optimistic concurrency column is needed.

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
> The `SagaClassMapping` has default mapping for the `CorrelationId` as a database generated primary key. If you use your own mapping, you must follow the same convention, otherwise there is a big chance to get deadlock exceptions in case of high throughput.

The repository is then created on the context factory:
```csharp
SagaDbContextFactory contextFactory = () => 
    new SagaDbContext<SagaInstance, SagaInstanceMap>(_connectionString);

var repository = new EntityFrameworkSagaRepository<SagaInstance>(
    contextFactory, optimistic: true); // true For Optimistic Concurrency, false is default for pessimistic
```

Lastly, the snippet below is _only needed for optimistic concurrency_, because the saga should retry processing if failure occurred when writing to the database. This snippet is adding [retry policies](../../usage/retries.md) middleware to the saga receive endpoint from the [first section](#specifying-saga-persistence).

```csharp
x.ReceiveEndpoint(host, "shopping_cart_state", e =>
{
    e.UseRetry(x => 
        {
            x.Handle<DbUpdateConcurrencyException>();
            x.Interval(5, TimeSpan.FromMilliseconds(100)));
        }); // Add the retry middleware for optimistic concurrency
    e.StateMachineSaga(sagaStateMachine, repository);
});
```

Hence, that if you have retry policy without an exception filter, it will also handle the concurrency exception, so explicit configuration is not required in this case.

### MongoDB

MongoDB is an easy to use saga repository, because setup is easy. There is no need for class mapping, the saga instances can be persisted easily using a MongoDB collection.

#### Concurrency

MongoDb saga persistence requires that saga instance classes implement `IVersionedSaga` interface. This interface has a `Version` property, which allows the saga persistence to handle optimistic concurrency.

#### Configuration and usage

```csharp
public class SagaInstance : SagaStateMachineInstance, IVersionedSaga
{
    public SagaInstance(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    protected SagaInstance() { }

    public string CurrentState { get; set; }
    public string ServiceName { get; set; }
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
}
```

The saga repository is created using the simple syntax:

```csharp
var database = new MongoClient("mongodb://127.0.0.1").GetDatabase("sagas");
var repository = new MongoDbSagaRepository<SagaInstance>(database);
```

Each saga instance will be placed in a collection specific to the instance type.

### NHibernate

NHibernate is a widely used ORM and it is supported by MassTransit for saga storage. The example below shows the code-first approach to using NHibernate for saga persistence.

#### Concurrency

NHibernate natively supports multiple concurrency handling mechanisms. The easiest is probably adding a `Version` property of type `int` to the saga instance class and map it to the column with the same name. NHibernate will use it by default.

#### Configuration and usage

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
    public int Version { get; set; } // for optimistic concurrency
}

public class SagaInstanceMap : SagaClassMapping<SagaInstance>
{
    public SagaInstanceMap()
    {
        Property(x => x.CurrentState);
        Property(x => x.ServiceName, x => x.Length(40));
        Property(x => x.Version); // for optimistic concurrency
    }
}
```

The `SagaClassMapping` base class maps the `CorrelationId` of the saga, and handles some of the basic bootstrapping of the class map. All other properties, including the property for the `CurrentState` (if you're using state machine sagas), must be mapped by the developer. Once mapped, the `ISessionFactory` can be created using NHibernate directly. From the session factory, the saga repository can be created.

> Important:
> The `SagaClassMapping` has default mapping for the `CorrelationId` as a database generated primary key. If you use your own mapping, you must follow the same convention, otherwise there is a big chance to get deadlock exceptions in case of high throughput.

```csharp
ISessionFactory sessionFactory = CreateSessionFactory();
var repository = new NHibernateSagaRepository<SagaInstance>(sessionFactory);
```

### Redis

Redis is a very popular key-value store, which is known for being very fast.

Redis does not support queries, therefore Redis saga persistence only supports correlation by id. If you try to use correlation by expressions, you will get a "not implemented" exception.

Saga persistence for Redis uses `StackExchange.Redis` library.

#### Redis client initialization

Redis saga repository is based on the popular `StackExchange.Redis` package and therefore requires `StackExchange.Redis.IDatabase` factory function as constructor parameter. For containerless initialization the code would look like:

```csharp
var redisConnectionString = "redis://localhost:6379";
var redis = ConnectionMultiplexer.Connect(redisConnectionString);

var repository = new RedisSagaRepository<SagaInstance>(() => redis.GetDatabase());
```

If you use a container, you can use the code like this (example for Autofac):

```csharp
var redisConnectionString = "redis://localhost:6379";
builder.RegisterInstance(ConnectionMultiplexer.Connect(redisConnectionString))
    .As<IConnectionMultiplexer>();
builder.Register<IDatabase>(c => c.Resolve<IConnectionMultiplexer>.GetDatabase());
builder.RegisterGeneric(typeof(RedisSagaRepository<>))
    .As(typeof(ISagaRepository<>)).SingleInstance();
```

#### Concurrency

Redis persistence supports optimistic and pessimistic concurrency. The default mode is optomistic concurrency.

In optimistic concurrency mode, Redis saga persistence does not acquire locking on the database record when writing it so potentially you can have write conflict in case the saga is updating its state frequently (hundreds of times per second). To resolve this, the saga instance can implement the `IVersionedSaga` interface and include the Version property:

```csharp
public int Version { get; set; }
```

When the version of the instance that is being updated is lower than the expected version, the saga repository will trow an exception and force the message to be retried, potentially resolving the issue.

Pessimistic concurrency can be used by specifying `optimistic: false` parameter in the repository constructor. It will instruct the repository to use Redis lock mechanism. During the message processing, saga instance in Redis will be locked and any concurrent attempts to execute any processing on the same instance will fail.

### Marten

[Marten][2] is an open-source library that provides an API to the PostgreSQL [JSONB storage][1], influenced by RavenDb client API. It allows to use PostgreSQL as schema-less NoSQL document storage. Unlike typical document databases, PostgreSQL JSONB storage provides you the ACID-compliant transactional store with full consistency.

To use Marten and PostgreSQL as saga persistence, you need to install `MassTransit.Marten` NuGet package and add some code.

First, your saga state class needs to mark the correlationId property with the `[Identity]` attribute. By this you inform Marten that correlationId will be used as the primary key.

```csharp
public class SampleSaga : ISaga
{
    [Identity]
    public Guid CorrelationId { get; set; }
    public string State { get; set; }
    public string SomeProperty { get; set; }
}
```

Then you need to initialize the document store and the repository. Repository needs the store as its constructor parameter.

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

Marten will create the necessary tables for you. This type of saga repository supports correlation by id and custom expressions.

#### Concurrency

Marten supports optimistic concurrency by using an eTag-like version field in the metadata. This means that the saga instance class does not need any additional fields for version.

There are two ways to use this feature:

1) Apply `[UseOptimisticConcurrency]` attribute to the saga instance class

```csharp
[UseOptimisticConcurrency]
public class SampleSaga : ISaga
{
    [Identity]
    public Guid CorrelationId { get; set; }
    ...
}
```

2. Configure the store to use optimistic concurrency for your saga instance class

 ```csharp 
 var store = DocumentStore.For(_ =>
 {
     // Adds optimistic concurrency checking to Issue
     _.Schema.For<SampleSaga>().UseOptimisticConcurrency(true);
 });
 ```

### DocumentDb

DocumentDb is the precessor of Azure CosmosDb and the DocumentDb API is still one of the main APIs for the NoSQL document-oriented persistence of CosmosDb. MassTransit supports saga persistence in CosmosDb by using both MongoDb API (using `MassTransit.MongoDb` package) and using DocumentDb API (using `MassTransit.DocumentDb` package).

DocumentDb requires that any document stored there has a property called `id`, to be used as the document identity. Saga instances have `CorrelationId` for the same purpose, so there are two ways to create your DocumentDb saga class, which can have different implications depending on your usage. ETag must also be present, which is used for optimistic concurrency. Please never set this property yourself, it managed 100% by document db.

#### First, the simple (out of box) functionality. Create your saga class:

```csharp
public class SampleSaga : IVersionedSaga
{
    public Guid CorrelationId { get; set; }
    public string ETag { get; set; }
    public string State { get; set; }
    public string SomeProperty { get; set; }
}

// And in your bus/saga configuration, you explicitly pass in the settings
var repository = new DocumentDbSagaRepository<SampleSaga>(documentDbClient, "sagaDatabase", JsonSerializerSettingsExtensions.GetSagaRenameSettings<SimpleSaga>());
```

The only restriction with this method is you might run into trouble if you are using Correlation Expressions that use the CorrelationId property. This is because when passing these expressions into DocumentDb's Create Query, it must have the `[JsonProperty("id")]` attribute instead of using the `JsonSerializerSettingsExtensions.GetSagaRenameSettings<...>()` rename.

#### So the second option for your saga class declaration is:

```csharp
public class SampleSaga : IVersionedSaga
{
    [JsonProperty("id")]
    public Guid CorrelationId { get; set; }
    [JsonProperty("_etag")]
    public string ETag { get; set; }
    public string State { get; set; }
    public string SomeProperty { get; set; }
}

// And in your bus/saga configuration, just follow the example below, no need to use the GetSagaRenameSettings<>()
```

And optionally, you can make your Saga inherit from the Azure DocumentDb class `Resource`, because.. well why not? It's saving to that store, so you might as well have all the properties there anyways.

#### Third option for saga class declaration:

```csharp
public class SampleSaga : IVersionedSaga, Resource
{
    [JsonProperty("id")] // This overrides the Resource [JsonProperty("id")], which exists on the Resource classes Id property. This means Id will be a null guid, so just always use CorrelationId instead
    public Guid CorrelationId { get; set; }
    // The Resource class has the [JsonProperty("_etag")] public string ETag {get;set;}, so we don't need to declare it here
    public string State { get; set; }
    public string SomeProperty { get; set; }
}

// And in your bus/saga configuration, just follow the example below, no need to use the GetSagaRenameSettings<>()
```

So my preference is option 3, or option 2. Option 1 is there to offer an option as backwards compatibility to existing functionality.

Instantiation of the DocumentDb saga repository could be done like this:

```csharp
var documentDbClient =  new DocumentClient(endpointUri, authKeyString);
var repository = new DocumentDbSagaRepository<SampleSaga>(documentDbClient, "sagaDatabase");
```

If you use a container, you can use the code like this (example for Autofac):

```csharp
builder.RegisterInstance(new DocumentClient(endpointUri, authKeyString))
    .As<IDocumentClient>();
builder.Register(c => 
        new DocumentDbSagaRepository(c.Resolve<IDocumentClient>(), "sagaDatabase"))
    .As<ISagaRepository<SampleSaga>>()
    .SingleInstance();
```

### Azure Service Bus

Azure Service Bus provides a feature called *message sessions*, to process multiple messages at once and to store some state on a temporary basis, which can be retrieved by some key.

The latter give us an ability to use this feature as saga state storage. Using message sessions as saga persistence, you can only use Azure Service Bus for both messaging and saga persistence purposes, without needing any additional infrastructure.

There is a limitation for using message sessions - this feature is not supported for AMQP transport.

You have to explicitly enable message sessions when configuring the endpoint, and use parameterless constructor to instantiate the saga repository.

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

As mentioned before, the message session allows storing and retrieving any state by some unique key. This means that this type of saga persistence only support correlation by id. So, similar to Redis saga persistence, you cannot use `CorrelateBy` to specify how to find the saga instance, but only `CorrelateById`.

### Dapper

Provides persistence for MSSQL using [Dapper][3].

Dapper.Contrib is used for inserts and updates. The methods are virtual, so if you'd rather write the SQL yourself it is supported.

If you do not write your own sql, the model requires you use the `ExplicitKey` attribute for the CorrelationId. And if you have properties that are not available as columns, you can use the `Computed` attribute to not include them in the generated SQL.

```csharp
public class SampleSaga : ISaga
{
    [ExplicitKey]
    public Guid CorrelationId { get; set; }
    public string Name { get; set; }
    public string State { get; set; }

    [Computed]
    public Expression<Func<SimpleSaga, ObservableSagaMessage, bool>> CorrelationExpression
    {
        get { return (saga, message) => saga.Name == message.Name; }
    }
}
```

#### Limitations
The tablename can only be the pluralized form of the class name. So `SampleSaga` would translate to table SampleSaga**s**. This applies even if you write your own SQL for updates and inserts.

The expressions you can use for correlation is somewhat limited. These types of expressions are handled:

```csharp
    x => x.CorrelationId == someGuid;
    x => x.IsDone;
    x => x.CorrelationId == someGuid && x.IsDone;
```
You can use multiple `&&` in the expression.

What you can not use is `||` and negations. So a bool used like this `x.IsDone` can only be handled as true and nothing else.

Dapper does not yet support strong naming, though it is being [worked][4] on.

Also this does not support dotnetcore yet.

[1]: https://www.postgresql.org/docs/9.5/static/functions-json.html
[2]: http://jasperfx.github.io/marten/
[3]: https://github.com/StackExchange/Dapper
[4]: https://github.com/StackExchange/Dapper/issues/889