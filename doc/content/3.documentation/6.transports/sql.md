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

The database transport:

- Stores messages, queues, topics, and subscriptions using tables, indices, and functions
- Requires no custom extensions or additional services
- Uses pure SQL via DbConnection, DbCommand, and DbDataReader (no Entity Framework required)
- Behaves like a true message broker, similar to RabbitMQ, Azure Service Bus, or Amazon SQS
    - Messages are locked, locks are automatically renewed, and messages are acknowledged/removed once successfully consumed
    - Competing consumer (load balancing) to scale out service instances
    - Delayed redelivery (second-level retry) is implemented at the transport layer, rescheduling messages and adding exception headers
- Uses PostgreSQL's `LISTEN`/`NOTIFY` channels reduce polling frequency enabling immediate message delivery

## Features

The database transport supports:

- Durable messages, stored as JSON, with all headers and metadata stored in separate columns
- Publish/subscribe using polymorphic, topic-based message routing
- Topic-to-topic and topic-to-queue subscriptions
- Multiple subscription types including _All_ (fan-out), _Routing Key_ (direct), and _Pattern_ (topic)
- Dead-letter (*_skipped*) and error sub-queues
- Message scheduling, including cancellation
- Delayed redelivery (second-level retry)
- Message priority, at the message level
- All consumer types, including consumers, sagas, state machines, and routing slips
- Transactional Outbox using Entity Framework Core

## Sample 

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

To configure the database options, the standard .NET options pattern can be used. The individual options can be specified, as shown below.

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
    "Db": "Server=localhost;Port=5432;user id=postgres;password=Password12!;database=my_app;",
  },
  "AllowedHosts": "*"
}
```

To automatically create the database, tables, roles, functions, and other related database elements, a hosted service is available. 

```csharp
services.AddPostgresMigrationHostedService(create: true, delete: false);
```

::alert{type="danger"}
Specifying `delete: true` is only recommended for unit tests!
::

To use an existing database (which may be the case with Azure SQL or Azure PostreSQL), you can skip database creation but still create all the tables and
functions/stored procedure required.

```csharp
services.AddPostgresMigrationHostedService(x =>
{
    x.CreateDatabase = false;
    x.CreateInfrastructure = true; // this is the default, but shown for completeness
});
```

::alert{type="info"}
For SQL Server, replace `AddPostgresMigrationHostedService` with `AddSqlServerMigrationHostedService`.
::

## Subscription Types

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
    m.SubscriptionType = TopicSubscriptionType.RoutingKey;
    m.RoutingKey = "8675309";
});
```

Messages can then be published with a _RoutingKey_ so that they are properly routed:

```csharp
await publishEndpoint.Publish(new CustomerUpdatedEvent(NewId.NextGuid()),
    x => x.SetRoutingKey("8675309"));
```

### Pattern

The `Pattern` subscription type is used to filter messages so that only messages with a regular expression matching the routing key are delivered to the destination.
When adding a pattern subscription, it's usually necessary to disable the automatic topology configuration so that an `All` subscription won't be 
added for the consumer.

```csharp
e.ConfigureConsumeTopology = false;

e.Subscribe<CustomerUpdatedEvent>(m =>
{
    m.SubscriptionType = TopicSubscriptionType.Pattern;
    m.RoutingKey = "^[A-Z]+$";
});
```

Messages can then be published with a _RoutingKey_ so that they are properly routed:

```csharp
await publishEndpoint.Publish(new CustomerUpdatedEvent(NewId.NextGuid()),
    x => x.SetRoutingKey("ABCDEFG"));
```
