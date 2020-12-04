# Scoped Filters

Most of the built-in filters are created and added to the pipeline during configuration. This approach is typically sufficient, however, there are scenarios where the filter needs access to other components at runtime.

Using a scoped filter, combined with a supported dependency injection container (either MSDI or Autofac), allows a new filter instance to be resolved from the container for each message. If a current scope is not available, a new scope will be created using the root container.

### Filter Classes

Scoped filters must be generic classes with a single generic argument for the message type. For example, a scoped consume filter would be defined as shown below.

```cs
public class TFilter<TMessage> :
    IFilter<ConsumeContext<TMessage>>
```

### Supported Filter Contexts

Scope filters are added using one of the following methods, which are specific to the filter context type.

| Type                         | Usage                                                     |
| ---------------------------- | --------------------------------------------------------- |
| `ConsumeContext<T>`          | `UseConsumeFilter(typeof(TFilter<>), context)`            |
| `SendContext<T>`             | `UseSendFilter(typeof(TFilter<>), context)`               |
| `PublishContext<T>`          | `UsePublishFilter(typeof(TFilter<>), context)`            |
| `ExecuteContext<TArguments>` | `UseExecuteActivityFilter(typeof(TFilter<>), context)`    |
| `CompensateContext<TLog>`    | `UseCompensateActivityFilter(typeof(TFilter<>), context)` |

More information could be found inside [middleware](README.md) section

### Usage

To create a `ConsumeContext<T>` filter and add it to the receive endpoint:

```cs
public class MyConsumeFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
{
    public MyConsumeFilter(IMyDependency dependency) { }
      
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next) { }
      
    public void Probe(ProbeContext context) { }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
      	//other configuration
        services.AddScoped<IMyDependency, MyDependency>(); //register dependency

        services.AddConsumer<MyConsumer>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ReceiveEndpoint("input-queue", e =>
                {
                    e.UseConsumeFilter(typeof(MyConsumeFilter<>), context); //generic filter

                    e.ConfigureConsumer<MyConsumer>();
                });
            });
        });
    }
}
```

To create a `SendContext<T>` filter and add it to the send pipeline:

```cs
public class MySendFilter<T> :
    IFilter<SendContext<T>>
    where T : class
{
    public MySendFilter(IMyDependency dependency) { }
      
    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next) { }
      
    public void Probe(ProbeContext context) { }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //other configuration
        services.AddScoped<IMyDependency, MyDependency>(); //register dependency
          
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
              cfg.UseSendFilter(typeof(MySendFilter<>), context); //generic filter
            });
        });
    }
}
```

### Combining Consume And Send/Publish Filters

A common use case with scoped filters is transferring data between the consumer. This data may be extracted from headers, or could include context or authorization information that needs to be passed from a consumed message context to sent or published messages. In these situations, there _may_ be some special requirements to ensure everything works as expected.

The following example has both consume and send filters, and utilize a shared dependency to communicate data to outbound messages.

```cs
public class MyConsumeFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
{
    public MyConsumeFilter(MyDependency dependency) { }
      
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next) { }
      
    public void Probe(ProbeContext context) { }
}

public class MySendFilter<T> :
    IFilter<SendContext<T>>
    where T : class
{
    public MySendFilter(MyDependency dependency) { }
      
    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next) { }
      
    public void Probe(ProbeContext context) { }
}

public class MyDependency 
{
    public string SomeValue { get; set; }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<MyDependency>();

        services.AddConsumer<MyConsumer>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseSendFilter(typeof(MySendFilter<>), context);

                cfg.ReceiveEndpoint("input-queue", e =>
                {
                    e.UseConsumeFilter(typeof(MyConsumeFilter<>), context);
                    e.ConfigureConsumer<MyConsumer>();
                });
            });
        });
    }
}
```

::: warning
When using the InMemoryOutbox with scoped publish or send filters, `UseMessageScope` (for MSDI) or `UseMessageLifetimeScope` (for Autofac) must be configured _before_ the InMemoryOutbox. If `UseMessageRetry` is used, it must come _before_ either `UseMessageScope` or `UseMessageLifetimeScope`.
:::

Because the InMemoryOutbox delays publishing and sending messages until after the consumer or saga completes, the created container scope will have been disposed. The `UseMessageScope` or `UseMessageLifetimeScope` filters create the scope before the InMemoryOutbox, which is then used by the consumer or saga and any scoped filters (consume, publish, or send).

The updated receive endpoint configuration using the InMemoryOutbox is shown below.

```cs
                cfg.ReceiveEndpoint("input-queue", e =>
                {
                    e.UseMessageRetry(r => r.Intervals(100, 500, 1000, 2000));
                    e.UseMessageScope(context);
                    e.UseInMemoryOutbox();

                    e.UseConsumeFilter(typeof(MyConsumeFilter<>), context);
                    e.ConfigureConsumer<MyConsumer>();
                });
```




