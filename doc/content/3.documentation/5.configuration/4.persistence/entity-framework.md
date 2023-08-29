# Entity Framework

[![alt NuGet](https://img.shields.io/nuget/v/MassTransit.EntityFrameworkCore.svg "NuGet")](https://nuget.org/packages/MassTransit.EntityFrameworkCore/)

An example saga instance is shown below, which is orchestrated using an Automatonymous state machine. The _CorrelationId_ will be the primary key, and _CurrentState_ will be used to store the current state of the saga instance. 

```csharp
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

::alert{type="warning"}
The `SagaClassMap` has a default mapping for the `CorrelationId` as the primary key. If you create your own mapping, you must follow the same convention, or at least make it a Clustered Index + Unique, otherwise you will likely experience deadlock exceptions and/or performance issues in high throughput scenarios.
::

```csharp
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

```csharp
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

## Configuration

Once the class map and associated _DbContext_ class have been created, the saga repository can be configured with the saga registration, which is done using the configuration method passed to _AddMassTransit_. The following example shows how the repository is configured for using Microsoft Dependency Injection Extensions, which are used by default with Entity Framework Core.

```csharp
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

### Database Engines
By default, MassTransit uses SQL Server locking statements to handle concurrency. It is important, however, if using PostgreSQL, MySQL or Sqlite that you specify this as part of the setup of the DbContextOptionsBuilder options.

The following shows an example for PostgreSQL
```csharp
services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

            r.AddDbContext<DbContext, OrderStateDbContext>((provider,builder) =>
            {
                builder.UseNpgsql(connectionString, m =>
                {
                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                });
            });

            r.UsePostgres();
        });
});
```

### Shared DbContext

A single `DbContext` can be registered in the container which can then be used to configure sagas that are mapped by the `DbContext`. For example, [Job Consumers](/documentation/patterns/job-consumers) needs three saga repositories, and the Entity Framework Core package includes the `JobServiceSagaDbContext` which can be configured using the `AddSagaRepository` method as shown below.

```csharp
services.AddDbContext<JobServiceSagaDbContext>(builder =>
    builder.UseNpgsql(Configuration.GetConnectionString("JobService"), m =>
    {
        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
        m.MigrationsHistoryTable($"__{nameof(JobServiceSagaDbContext)}");
    }));

services.AddMassTransit(x =>
{
    x.AddSagaRepository<JobSaga>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<JobServiceSagaDbContext>();
            r.LockStatementProvider = new PostgresLockStatementProvider();
        });
    x.AddSagaRepository<JobTypeSaga>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<JobServiceSagaDbContext>();
            r.LockStatementProvider = new PostgresLockStatementProvider();
        });
    x.AddSagaRepository<JobAttemptSaga>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<JobServiceSagaDbContext>();
            r.LockStatementProvider = new PostgresLockStatementProvider();
        });

    // other configuration, such as consumers, etc.
});
```

The above code using the standard Entity Framework configuration extensions to add the _DbContext_ to the container, using PostgreSQL. Because the job service state machine receive endpoints are configured by _ConfigureJobServiceEndpoints_, the saga repositories must be configured separately. The _AddSagaRepository_ method is used to register a repository for a saga that has already been added, and uses the same extension methods as the _AddSaga_ and _AddSagaStateMachine_ methods.

Once configured, the job service sagas can be configured as shown below.

```csharp
cfg.ServiceInstance(options, instance =>
{
    instance.ConfigureJobServiceEndpoints(js =>
    {
        js.ConfigureSagaRepositories(context);
    });
});
```

:sample{sample="job-consumer"}

The sample above is a working example of this configuration style.

### Multiple DbContext

Multiple `DbContext` can be registered in the container which can then be used to configure sagas that are mapped by the `DbContext` and injected into other components. Calling the `AddDbContext` extension method will register a scoped `DbContext` by default. For simple scenarios where there is a single `DbContext` this will work. However, in scenarios where there is at least one other `DbContext` the dotnet command that generates Entity Framework migrations will not work. To resolve this issue, you'll need to perform the following steps:

1.  Make sure that all `DbContext` has a constructor that takes `DbContextOptions<TOptions>` instead of `DbContextOptions`.

2.  Run the Entity Framework Core command to create your migrations as shown below.

    ```bash
    dotnet ef migrations add InitialCreate -c JobServiceSagaDbContext
    ```

3.  Run the Entity Framework Core command to sync with the database as shown below.
 
    ```bash
    dotnet ef database update -c JobServiceSagaDbContext
    ```


