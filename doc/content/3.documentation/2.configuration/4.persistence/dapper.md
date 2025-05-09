# Dapper

[![alt NuGet](https://img.shields.io/nuget/v/MassTransit.DapperIntegration.svg "NuGet")](https://nuget.org/packages/MassTransit.DapperIntegration/)

[Dapper][1] is a [super lightweight Micro-ORM][2] usable for saga persistence with multiple RDBMS providers. Support is included for SQL Server and Postgres, and JobConsumers are also supported out of the box.

Using the Dapper repository is straightforward.  `CorrelationId` is assumed to be the primary key for all saga instances, and does not require explicit decoration.
```csharp
public class OrderState :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }

    public DateTime? OrderDate { get; set; }
}
```

## Configuration

To configure Dapper as the repository for a saga, use the code shown below using the _AddMassTransit_ container extension, and configure the repository further.  The default implementations provide configuration options for common settings, as well as allowing complete replacements of the SQL formatters to support additional engines, or entire custom implementations of the repository for non-trivial models.  These options can be configured on a per-saga basis, and there are no requirements that different sagas must use the same database engine.

### Inline Configuration

The most familiar way to register dependencies with MassTransit, the Dapper configuration will feel familiar:
```csharp
container.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository(conf => {
            // either will handle a substantial portion of configuration,
            // but can be skipped in favor of building everything manually
            conf.UseSqlServer(connectionString: "...");
            conf.UsePostgres(connectionString: "...");

            // default is IsolationLevel.Serializable
            conf.UseIsolationLevel(IsolationLevel.ReadCommitted);

            // default is 'SagaName' + 's', but can be overridden with
            // [Dapper.Contrib.TableAttribute] on the saga, or specified:
            conf.UseTableName("dbo.Orders");

            // default is 'CorrelationId', but can be overridden with
            // [KeyAttribute] or [ExplicitKeyAttribute] (from Dapper.Contrib),
            // or specified:
            conf.UseIdColumnName("OrderId");    // NOTE: composite keys are NOT supported by default!
        });
});
```

### Advanced Configuration

If the common configuration options aren't enough for your application, it's now possible to swap entire implementations during the configuration. When consuming a saga message, a few steps happen, each completely pluggable:

 1. A `DbConnection` is resolved via `DapperOptions<TSaga>.DbConnectionProvider(IServiceProvider services)`.
 2. A transaction is started at the level specified by `DapperOptions<TSaga>.IsolationLevel`.
 3. A `DatabaseContextFactory<TSaga>` factory is resolved via `DapperOptions<TSaga>.ContextFactoryProvider`.
 4. The connection and transaction are passed to the context factory, to resolve an instance of `DatabaseContext<TSaga>`
 5. The default context will resolve an implementation of `ISagaSqlFormatter<TSaga>` via `DapperOptions<TSaga>.SqlBuilderProvider(IServiceProvider services)`

```csharp

container.AddMassTransit(cfg => 
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository(conf => {
            // if your saga needs an entirely different database connection
            // a custom connection factory can be specified.  transactions will
            // be created off this connection instead, and will not participate
            // in transactional consume contexts. this will override either
            // UseSqlServer() or UsePostgres().
            conf.UseDbConnectionProvider(sp => new OracleConnection("..."));

            // if your saga needs a different SQL syntax, a different formatter 
            // can be used. default implementations include generic syntax for
            // SQL Server and Postgres and will work in most cases
            conf.UseSqlFormatter(sp => sp.GetRequiredService<OrderStateSqlFormatter>());

            // if your saga needs an entirely custom repository implementation, 
            // the entire repository factory can be specified manually.
            conf.UseContextFactory(sp => sp.GetRequiredService<OrderStateContext>());

        });
});

```

### IOptions configuration

Inline configuration is not required for most options, and can be directly bound to configuration or set with `IConfigureOptions<>`.  This can make it much easier to resolve values or implementations during initialization time.  Any values that are not set with `UseXXX()` methods inside the configuration callback can be set directly on `DapperOptions<TSaga>`.

* The example below shows how a connection string might be set by an existing "application settings" object:
```csharp

// configuration can be set with IConfigureOptions<>:
services.AddTransient<IConfigureOptions<DapperOptions<OrderState>>, OrderStateConfiguration>();
services.AddMassTransit(bus => 
{
    bus.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository();   // no further configuration is actually needed
});

public class OrderStateConfiguration : IConfigureOptions<DapperOptions<OrderState>>
{
    private readonly AppConfiguration _conf;

    public OrderStateConfiguration(AppConfiguration conf) 
        => _conf = conf;

    public void Configure(DapperOptions<OrderState> options)
    {
        options.ConnectionString = _conf.OrdersConnectionString;
        options.TableName = _conf.OrdersTableName;
        options.IdColumnName = _conf.OrdersIdColumn;

        options.Provider = DatabaseProviders.SqlServer;
    }
}
```

* The example below shows what binding to a configuration section might look like:
```csharp

services.AddOptions<DapperOptions<OrderState>>().BindConfiguration("OrderStateOptions");
services.AddMassTransit(bus =>
{
    bus.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository();
});

// ----- appsettings.json -----
"OrderStateOptions": {
  "ConnectionString": "...",
  "Provider": "SqlServer",
  "TableName": "other_table"
}
```

* Any combination an be used:
```csharp
services
  .AddOptions<DapperOptions<OrderState>>()
  .BindConfiguration("OrderStateOptions")
  .Configure(cfg => cfg.IdColumnName = "OrderId");

services.AddMassTransit(bus =>
{
    bus.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository(conf => {
            conf.UseSqlFormatter(_ => new OrderStateSqlFormatter());
        });
});

```

## Correlation Expressions

The expressions you can use for correlation is somewhat limited, but it does support `&&`, `!`, `!=`, `==`, `<`, `<=`, `>`, `>=`, and implicit boolean True.  Expressions with "OR" (`||`) and any expressions with method calls (`x => x.Name.ToLower() == username`) are not supported at this time.

```csharp
x => x.CorrelationId == someGuid;
x => x.IsDone;
x => ! x.IsDone;
x => x.CorrelationId == someGuid && x.IsDone;
x => x.ExpirationDate <= now && !x.Invalid;
```

## JobConsumers

### Configuration

JobConsumers are supported with the Dapper provider now:

```csharp
services.AddMassTransit(bus =>
{
    bus.AddJobSagaStateMachines()
        .DapperRepository(opt => opt.UseSqlServer(_connectionString));
});
```

The inline configuration allows you to specify the ContextFactory for each of the 3 JobConsumer saga types:
```csharp
services.AddMassTransit(bus =>
{
    bus.AddJobSagaStateMachines()
        .DapperRepository(opt => {
            opt.UseJobContextFactory(_ => (connection, transaction) => new CustomJobSagaContext(connection, transaction));
            opt.UseJobTypeContextFactory(_ => (connection, transaction) => new CustomJobTypeSagaContext(connection, transaction));
            opt.UseJobAttemptContextFactory(_ => (connection, transaction) => new CustomJobAttemptSagaContext(connection, transaction));
        });
});
```

or configure specific details like any other saga:
```csharp
services.AddOptions<DapperOptions<JobSaga>>().Configure(opt =>
{
    opt.ConnectionString = _connectionString;
    opt.Provider = DatabaseProviders.SqlServer;
});

services.AddOptions<DapperOptions<JobTypeSaga>>().Configure(opt =>
{
    opt.ConnectionString = _connectionString;
    opt.Provider = DatabaseProviders.SqlServer;
});

services.AddOptions<DapperOptions<JobAttemptSaga>>().Configure(opt =>
{
    opt.ConnectionString = _connectionString;
    opt.Provider = DatabaseProviders.SqlServer;
});

services.AddMassTransit(bus =>
{
    bus.AddJobSagaStateMachines()
        .DapperRepository();
});

```

### 

### Caveats

 * The included implementations for JobConsumers will store some properties as JSON.  
 * Correlation expressions aren't supported for JobConsumers.

## Upgrading from previous versions

Any code which uses `.DapperRepository(_connectionString)` with no additional setup or configuration should behave the same as before, using `DapperDatabaseContext<TSaga>`.  All newer code will use `SagaDatabaseContext<TSaga>` by default.

Convert old implementations by calling `UseSqlServer()` with the same connection string -- no further changes are required:

```csharp
services.AddMassTransit(bus =>
{
    // OLD:
    bus.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository(_connectionString);

    // NEW:
    bus.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .DapperRepository(conf => conf.UseSqlServer(_connectionString));  // (the legacy context assumed SqlServer only)
});
```

The legacy configuration may be removed in later versions.  It is recommended to migrate to the newer API when practical.

[1]: https://dapper-tutorial.net/
[2]: https://github.com/StackExchange/Dapper
