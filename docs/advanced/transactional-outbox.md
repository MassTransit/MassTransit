# Transactional Outbox

It is common that a service may need to combine database writes with publishing events and/or sending commands. And in this scenario, it is usually desirable to do this atomically in a transaction. However, message brokers typically do not participate in transactions. Even if a message broker did support transactions, it would require two-phase commit (2PC) which should be avoided whenever possible.

> While MassTransit has long provided an [in-memory outbox](/articles/outbox), there has often been criticism that it isn't a _real_ outbox. And while I have proven that it works, is reliable, and is extremely fast (broker message delivery speed), it does require care to ensure operations are idempotent and when an idempotent operation is detected events are republished. The in-memory outbox also does not function as an _inbox_, so exactly-once message delivery is not supported.

The Transactional Outbox has two main components:

- The **Bus Outbox** works within a container scope (such as the scope created for an ASP.NET Controller) and adds published and sent messages to the specified `DbContext`. Once the changes are saved, the messages are available to the delivery service which delivers them to the broker.

- The **Consumer Outbox** is a combination of an _inbox_ and an _outbox_. The _inbox_ is used to keep track of received messages to guarantee  exactly-once consumer behavior. The _outbox_ is used to store published and sent messages until the consumer completes successfully. Once completed, the stored messages are delivered to the broker after which the received message is acknowledged. The Consumer Outbox works with all consumer types, including Consumers, Sages, and Courier Actvities.

Either of these components can be used independently or both at the same time.

### Bus Outbox Behavior

Normally when messages are published or sent they are delivered directly to the message broker:

![Delivery to Broker](/write-to-broker.png "Delivery to Broker")

When the bus outbox is configured, the scoped interfaces are replaced with versions that write to the outbox. Since `ISendEndpointProvider` and `IPublishEndpoint` are registered as scoped in the container, they are able to share the same scope as the `DbContext` used by the application. 

![Delivery to Outbox](/write-to-outbox.png "Delivery to Outbox")

Once the changed are saved in the `DbContext` (typically by the application calling `SaveChangesAsync`), the messages will be written to the database as part of the transaction and will be available to the delivery service.

The delivery service queries the `OutboxMessage` table for messages published or sent via the Bus Outbox, and attempt to deliver any messages found to the message broker.

![Delivery to Broker](/outbox-to-broker.png "Delivery to Broker")

The delivery service uses the _OutboxState_ table to ensure that messages are delivered to the broker in the order they were published/sent. The _OutboxState_ table is also used to lock messages so that multiple instances of the delivery service can coexist without conflict.

### Consumer Outbox Behavior

Normally, when messages are published or sent by a consumer or one of its dependencies they are delivered directly to the message broker:

![Regular Consumer Behavior](/consumer-regular.png "Regular Consumer Behavior")

When the outbox is configured, the behavior changes. As a message is received, the _inbox_ is used to lock the message by `MessageId`.

![Consumer Inbox](/consumer-inbox.png "Consumer Inbox")

When the consumer publishes or sends a message, instead of being delivered to the broker it is stored in the _OutboxMessage_ table.

![Inbox to Outbox](/inbox-outbox.png "Inbox to Outbox")

Once the consumer completes and the messages are saved to the outbox, those messages are delivered to the message broker in the order they were produced. 

![Deliver Outbox to Broker](/inbox-outbox-broker.png "Deliver Outbox to Broker")

If there are issues delivering the messages to the broker, message retry will continue to attempt message delivery.

### Entity Framework Outbox

The Transactional Outbox for Entity Framework Core uses three tables in the `DbContext` to store messages that are subsequently delivered to the message broker.

| Table            | Description  |
| -----------------|-------|
| InboxState       | Tracks received messages by `MessageId` for each endpoint |
| OutboxMessage    | Stores messages published or sent using `ConsumeContext`, `IPublishEndpoint`, and `ISendEndpointProvider` |
| OutboxState      | Tracks delivery of outbox messages by the delivery service (similar to _InboxState_ but for message sent outside of a consumer via the bus outbox)  |

### Configuration

> The code below is based upon the [sample application](https://github.com/MassTransit/Sample-Outbox)

The outbox components are included in the `MassTransit.EntityFrameworkCore` NuGet packages. The code below configures both the bus outbox and the consumer outbox using the default settings. In this case, PostgreSQL is the database engine.

```cs
x.AddEntityFrameworkOutbox<RegistrationDbContext>(o =>
{
	// configure which database lock provider to use (Postgres, SqlServer, or MySql)
	o.UsePostgres();

	// enable the bus outbox
    o.UseBusOutbox();
});
```

To configure the _DbContext_ with the appropriate tables, use the extension methods shown below:

```cs
public class RegistrationDbContext :
    DbContext
{
    public RegistrationDbContext(DbContextOptions<RegistrationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
```

To configure the outbox on a receive endpoint, configure the receive endpoint as shown below. The configuration below uses a `SagaDefinition` to configure the receive endpoint, which is added to MassTransit along with the saga state machine.

```cs
public class RegistrationStateDefinition :
    SagaDefinition<RegistrationState>
{
    readonly IServiceProvider _provider;

    public RegistrationStateDefinition(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<RegistrationState> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));

        endpointConfigurator.UseEntityFrameworkOutbox<RegistrationDbContext>(_provider);
    }
}
```

The definition is added with the saga state machine:

```cs
x.AddSagaStateMachine<RegistrationStateMachine, RegistrationState, RegistrationStateDefinition>()
    .EntityFrameworkRepository(r =>
    {
        r.ExistingDbContext<RegistrationDbContext>();
        r.UsePostgres();
    });
```

The Entity Framework outbox adds a hosted service which removes delivered _InboxState_ entries after the _DuplicateDetectionWindow_ has elapsed. The Bus Outbox includes an additional hosted service that delivers the outbox messages to the broker.

### Configuration Options

The available outbox settings are listed below.

| Setting            | Description  |
| -----------------|-------|
| DuplicateDetectionWindow       | The amount of time a message remains in the inbox for duplicate detection (based on MessageId) |
| IsolationLevel    | The transaction isolation level to use (Serializable by default) |
| LockStatementProvider    | The lock statement provider, needed to execute pessimistic locks. Is set via `UsePostgres`, `UseSqlServer` (the default), or `UseMySql` |
| QueryDelay    | The delay between queries once messages are no longer available. When a query returns messages, subsequent queries are performed until no messages are returned after which the QueryDelay is used. |
| QueryMessageLimit    | The maximum number of messages to query from the database at a time |
| QueryTimeout    | The database query timeout |

The bus outbox includes some additional settings:

| Setting            | Description  |
| -----------------|-------|
| MessageDeliveryLimit       | The number of messages to deliver at a time from the outbox to the broker |
| MessageDeliveryTimeout    | Transport Send timeout when delivering messages to the transport |
| DisableDeliveryService()    | Disable the outbox message delivery service, removing the hosted service from the service collection |
