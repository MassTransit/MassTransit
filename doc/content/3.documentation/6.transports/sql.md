---
navigation.title: SQL/DB
---

# SQL Database Transport

In the realm of distributed systems and message-oriented architectures, a reliable and efficient message transport is a crucial aspect.

PostgreSQL and Microsoft SQL Server are renowned and feature-rich relational database management systems. When combined with the power of
MassTransit, these database engines emerge as a formidable choice for implementing a robust and scalable message transport.

By leveraging either of these databases as the underlying message storage and delivery mechanism, developers can harness the reliability, durability,
and transactional capabilities of the database, while benefiting from MassTransit's extensive support for message-based communication patterns.

This integration presents an enticing proposition for building resilient and high-performance distributed systems that can seamlessly handle
complex message flows and enable reliable communication between components.

## Details

The SQL transport:

- Stores messages, queues, topics, and subscriptions using tables, indices, and functions/stored procedures
- Requires no custom extensions or additional services
- Uses pure SQL via DbConnection, DbCommand, and DbDataReader (no Entity Framework required)
- Behaves like a true message broker, similar to RabbitMQ, Azure Service Bus, or Amazon SQS
    - Messages are locked, locks are automatically renewed, and messages are acknowledged/removed once successfully consumed
    - Competing consumer (load balancing) to scale out service instances
    - Delayed redelivery (second-level retry) is implemented at the transport layer, rescheduling messages and adding exception headers
- Uses PostgreSQL's `LISTEN`/`NOTIFY` channels to reduce polling frequency while still enabling immediate message delivery

### Features

The SQL transport supports:

- Durable messages, stored as JSON, with headers and metadata stored in separate columns
- Publish/subscribe messaging using polymorphic, topic-based routing
- Topic-to-topic and topic-to-queue subscriptions, enabling sophisticated message routing options
- Multiple subscription types including _All_ (fan-out), _Routing Key_ (direct), and _Pattern_ (topic)
- Dead-letter (*_skipped*) and error sub-queues with functions to move messages back into the main queue
- Message scheduling, including cancellation
- Delayed redelivery (second-level retry)
- Message priority, at the message level
- Partitioned message consumption, enabling fair message consumption across tenants, customers, etc. and ordered message delivery
- Supports all consumer types, including consumers, sagas, state machines, and routing slips
- Transactional Outbox using Entity Framework Core
- [Web-based UI](https://github.com/filipbekic01/resqueue) simplifies managing SQL and PostgreSQL messaging transports through a straightforward web interface

### Sample

:sample{sample="sql-transport"}

## Configuration

The SQL transport is configured with `UsingPostgres` or `UsingSqlServer`.

### PostgreSQL

```csharp
services.AddMassTransit(x =>
{
    x.AddSqlMessageScheduler();
    
    x.UsingPostgres((context, cfg) =>
    {
        cfg.UseSqlMessageScheduler();
    
        cfg.ConfigureEndpoints(context);
    });
});
```

### SQL Server

```csharp
services.AddMassTransit(x =>
{
    x.AddSqlMessageScheduler();
    
    x.UsingSqlServer((context, cfg) =>
    {
        cfg.UseSqlMessageScheduler();
    
        cfg.ConfigureEndpoints(context);
    });
});
```

## SqlTransportOptions

To configure the SQL transport options, the standard .NET options pattern should be used.

### Connection String

A standard connection string can be used to configure the SQL transport. In the example below, the configured connection string is retrieved and set
on the `SqlTrasnportOptions`.

```csharp
var connectionString = builder.Configuration.GetConnectionString("Db");

builder.Services.AddOptions<SqlTransportOptions>()
    .Configure(options =>
    {
        options.ConnectionString = connectionString;
    });
```

In the `appsettings.json`, the connection string should be configured. For PostgreSQL this may be something like:

```json
{
    "ConnectionStrings": {
        "Db": "Server=localhost;Port=5432;user id=postgres;password=Password12!;database=my_app;"
    },
    "AllowedHosts": "*"
}
```

### Options

Additionally, individual options can be specified, as shown below. This might be the case when you want to change the schema name or the role created by the
migration script. If the username and password used by the application do not have administrative rights to the database, a separate admin username and password
can also be specified.

```csharp
services.AddOptions<SqlTransportOptions>().Configure(options =>
{
    options.Host = "localhost";
    options.Database = "sample";
    options.Schema = "transport"; // the schema for the transport-related tables, etc. 
    options.Role = "transport";   // the role to assign for all created tables, functions, etc.
    options.Username = "masstransit";  // the application-level credentials to use
    options.Password = "H4rd2Gu3ss!";
    options.AdminUsername = builder.Username; // the admin credentials to create the tables, etc.
    options.AdminPassword = builder.Password;
});
```

:::alert{type="info"}
If the `AdminUsername` and `AdminPassword` are not specified, the `Username` and `Password` are used instead and may need elevated permissions to
allow creation of the database and/or infrastructure.
:::

### Migrations

To automatically create the database, tables, roles, functions, and other related database elements, a hosted service is available. The migration hosted
service should be added **BEFORE** `AddMassTransit` in the configuration to ensure the database has been created/configured before starting the bus.

```csharp
services.AddPostgresMigrationHostedService();
// OR
services.AddSqlServerMigrationHostedService();
```

To use an existing database (which may be the case with Azure SQL or Azure PostreSQL), you can skip database creation but still create all the tables and
functions/stored procedure required.

```csharp
services.AddPostgresMigrationHostedService(x =>
{
    x.CreateDatabase = false;
    x.CreateInfrastructure = true; // this is the default, but shown for completeness
});
```

::alert{type="danger"}
Specifying `DeleteDatabase = true` is only recommended for unit tests!
::

::alert{type="info"}
For SQL Server, replace `AddPostgresMigrationHostedService` with `AddSqlServerMigrationHostedService`.
::

## Topic Subscriptions

Several topic and queue subscription types are supported.

### All

By default, subscriptions are created with the `All` subscription type so that all messages
published and/or sent to the topic are delivered to the destination (either a queue or another topic).

### Routing Key

The `RoutingKey` subscription type is used to filter messages so that only messages with a matching routing key are delivered to the destination.
When adding a routing key subscription, it's usually necessary to disable the automatic topology configuration so that an `All` subscription won't be
added for the consumer.

```csharp
e.ConfigureConsumeTopology = false;

e.Subscribe<CustomerUpdatedEvent>(m =>
{
    m.SubscriptionType = SqlSubscriptionType.RoutingKey;
    m.RoutingKey = "8675309";
});
```

Messages can then be published with a _RoutingKey_ so that they are properly routed:

```csharp
await publishEndpoint.Publish(new CustomerUpdatedEvent(NewId.NextGuid()),
    x => x.SetRoutingKey("8675309"));
```

### Pattern

The `Pattern` subscription type is used to filter messages so that only messages with a regular expression matching the routing key are delivered to the
destination.
When adding a pattern subscription, it's usually necessary to disable the automatic topology configuration so that an `All` subscription won't be
added for the consumer.

```csharp
e.ConfigureConsumeTopology = false;

e.Subscribe<CustomerUpdatedEvent>(m =>
{
    m.SubscriptionType = SqlSubscriptionType.Pattern;
    m.RoutingKey = "^[A-Z]+$";
});
```

Messages can then be published with a _RoutingKey_ so that they are properly routed:

```csharp
await publishEndpoint.Publish(new CustomerUpdatedEvent(NewId.NextGuid()),
    x => x.SetRoutingKey("ABCDEFG"));
```

## Partitioned Queues

The SQL transport support message-level partition keys and messages can be consumed by partition key. This promotes fairness in how messages are delivered,
particularly in customer- or tenant-based applications to avoid an individual customer or tenant from blocking others due to high message volume. Consuming by
partition key can limit the number of messages consumed per partition key which evens out message delivery.

### Set Partition Key

Messages published or sent can specify the partition key with the `Publish` or `Send` call as shown.

```csharp
await publishEndpoint.Publish(new CustomerUpdatedEvent(NewId.NextGuid()),
    x => x.SetPartitionKey("CustomerA"));
```

Messages can also be configured to automatically set the partition key based on the message content by configuring a send convention during bus configuration.

```csharp
x.UsingSqlServer((context, cfg) => 
{
    cfg.SendTopology.UsePartitionKeyFormatter<CustomerUpdatedEvent>(x => x.Message.CustomerId);
});
```

Typically, it's easier to combine the message convention configuration into an extension methods and use that when configuring the bus:.

```csharp
public static class MessageConventionExtensions
{
    public static void UseMessagePartitionKeyFormatters(this IBusFactoryConfigurator cfg)
    {
        cfg.SendTopology.UsePartitionKeyFormatter<CustomerCreatedEvent>(x => x.Message.CustomerId);
        cfg.SendTopology.UsePartitionKeyFormatter<CustomerUpdatedEvent>(x => x.Message.CustomerId);
        cfg.SendTopology.UsePartitionKeyFormatter<CustomerDeletedEvent>(x => x.Message.CustomerId);
    }
}
```

Then, use the extension method when configuring the bus.

```csharp
x.UsingSqlServer((context, cfg) => 
{
    cfg.UseMessagePartitionKeyFormatters();
});
```

#### Global Topology

To configure transport-specific topology conventions at a global level using `GlobalTopology`, the appropriate conventions must be added. For example, to globally configure a _PartitionKey_ formatter for a base interface on a message contract:

```cs
GlobalTopology.Send.TryAddConvention(new PartitionKeySendTopologyConvention());

GlobalTopology.Send.UsePartitionKeyFormatter<ICanHasPartitionKey>(x => x.Message.PartitionKey.ToString());
```

### Set Receive Mode

The SQL transport supports multiple receive modes when configuring a receive endpoint. To enable partitioned delivery, one of the partitioned receive modes
must be configured.

| Receive Mode                 | Description                                                                                        |
|------------------------------|----------------------------------------------------------------------------------------------------|
| Normal                       | Standard priority-first FIFO (first-in, first-out) order                                           |
| Partitioned                  | Priority-first FIFO with only one message per PartitionKey concurrently                            | 
| PartitionedConcurrent        | Priority-first FIFO with up to `ConcurrentDeliveryLimit` messages per PartitionKey concurrently    | 
| PartitionedOrdered           | Explicit in-order FIFO  with one message per PartitionKey concurrently                             | 
| PartitionedOrderedConcurrent | Explicit in-order FIFO with up to `ConcurrentDeliveryLimit` messages per PartitionKey concurrently | 

There are a few notable aspects of these receive modes, including:

- When using a partitioned receive mode, messages are partitioned across **ALL** scaled out consumer instances. This delivery mechanism is unique to the
  SQL transport and enables scaling across high node counts and prevents a single partition key from saturating multiple consumer instances.
- Ordered receive modes are guaranteed to be in order, even when performing message redelivery or scheduling messages for future consumption. For example, if
  message 1 is scheduled for two minutes in the future, and message 2 and 3 with the same partition key are published any time after message 1, messages 2 and 3
  will _only_ be consumed after message 1 has been consumed.

The receive mode can be set when configuring the receive endpoint, or it can be added to the consumer endpoint configuration as shown.

```csharp
x.AddConsumer<CustomerCrudConsumer>()
    .Endpoint(e => e.AddConfigureEndpointCallback(cfg =>
    {
        if (cfg is ISqlReceiveEndpointConfigurator sql)
            sql.SetReceiveMode(SqlReceiveMode.Partitioned);
    }));
```

When settings a concurrent receive mode, the _ConcurrentDeliveryLimit_ should also be specified. This is useful when using a batch consumer.

```csharp
x.AddConsumer<BulkUpdateConsumer>(c => c.Options<BatchOptions>(o =>
{
    o.GroupBy<BulkUpdateMessage, string>(m => m.PartitionKey());
    o.SetConcurrencyLimit(10);
    o.SetMessageLimit(10);
    o.SetTimeLimit(ms: 10);
}))
.Endpoint(e => e.AddConfigureEndpointCallback(cfg =>
{
    if (cfg is ISqlReceiveEndpointConfigurator sql)
    {
        sql.SetReceiveMode(SqlReceiveMode.PartitionedConcurrent);
        sql.ConcurrentDeliveryLimit = 10;
    }
}));
```

### Job Sagas

When using the SQL transport with the job saga state machines, use the partitioned receive mode for the most reliable performance and concurrency. There are
two convenience methods that ensure the transport and sagas are properly configured: `SetPartitionedReceiveMode` and `UseJobSagaPartitionKeyFormatters`.

The method usage is shown below.

```csharp
services.AddMassTransit(x =>
{
    x.AddSqlMessageScheduler();

    x.AddJobSagaStateMachines()
        .SetPartitionedReceiveMode() // set job saga endpoints to partitioned
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<JobServiceSagaDbContext>();
            r.UsePostgres();
        });
    
    x.UsingPostgres((context, cfg) =>
    {
        cfg.UseSqlMessageScheduler();
        cfg.UseJobSagaPartitionKeyFormatters(); // partition key conventions
    
        cfg.ConfigureEndpoints(context);
    });
});
```

`UseJobSagaPartitionKeyFormatters` configures the partition key conventions so that the `PartitionKey` property is automatically set for the messages used by
the job saga state machines and consumers.
