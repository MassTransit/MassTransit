# Entity Framework Core

[MassTransit.EntityFrameworkCore](https://www.nuget.org/packages/MassTransit.EntityFrameworkCore)

An example saga instance is shown below, which is orchestrated using an Automatonymous state machine. The _CorrelationId_ will be the primary key, and _CurrentState_ will be used to store the current state of the saga instance. 

```cs
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

::: warning Important
The `SagaClassMap` has a default mapping for the `CorrelationId` as the primary key. If you create your own mapping, you must follow the same convention, or at least make it a Clustered Index + Unique, otherwise you will likely experience deadlock exceptions and/or performance issues in high throughput scenarios.
:::

```cs
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

```cs
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

### Container Integration

Once the class map and associated _DbContext_ class have been created, the saga repository can be configured with the saga registration, which is done using the configuration method passed to _AddMassTransit_. The following example shows how the repository is configured for using Microsoft Dependency Injection Extensions, which are used by default with Entity Framework Core.

```cs
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

### Custom schemas

#### Single schema

In case there is a custom schema set up in your database and you are relying on the user credentials from the `ConnectionString` to access correct schema you need to define a custom implementation of `SqlLockStatementProvider` where you can define that custom schema.

```cs
public class CustomSqlLockStatementProvider : SqlLockStatementProvider
{
    const string DefaultSchemaName = "custom_schema";

    public CSqlLockStatementProvider(bool enableSchemaCaching = true)
        : base(DefaultSchemaName, new SqlServerLockStatementFormatter(), enableSchemaCaching)
    {
    }
} 
```

And then use it in the configuration.

```cs
services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion
            
            r.LockStatementProvider = new CustomSqlLockStatementProvider();

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

#### Multiple schemas

In case there are multiple schemas defined for your models you need to define them in code, and there are a few different ways to do that.
**https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations#table-schema**


#### Single DbContext

> New in 7.0.5

A single `DbContext` can be registered in the container which can then be used to configure sagas that are mapped by the `DbContext`. For example, [Job Consumers](/advanced/job-consumers) need three saga repositories, and the Entity Framework Core package includes the `JobServiceSagaDbContext` which can be configured using the `AddSagaRepository` method as shown below.

```cs
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

```cs
cfg.ServiceInstance(options, instance =>
{
    instance.ConfigureJobServiceEndpoints(js =>
    {
        js.ConfigureSagaRepositories(context);
    });
});
```

The [Job Consumers](https://github.com/MassTransit/Sample-JobConsumers) sample is a working version of this configuration style.


#### Multiple DbContext

Multiple `DbContext` can be registered in the container which can then be used to configure sagas that are mapped by the `DbContext` and injected into other components. Calling the ```AddDbContext``` extension method will register a scoped ```DbContext``` by default. For simple scenarios where there is a single ```DbContext``` this will work. However, in scenarios where there is at least one other ```DbContext``` the dotnet command that generates Entity Framework migrations will not work. To resolve this issue, you'll need to perform the following steps:
1. Make sure that all ```DbContext``` has a constructor that takes ```DbContextOptions<TOptions>``` instead of ```DbContextOptions```.

2. Run the Entity Framework Core command to create your migrations as shown below.

```cs
dotnet ef migrations add InitialCreate -c JobServiceSagaDbContext
```

3. Run the Entity Framework Core command to sync with the database as shown below.
 
 ```cs
 dotnet ef database update -c JobServiceSagaDbContext
 ```


