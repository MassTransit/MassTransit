# Scoped Filters

By default filters are singleton, but using container integration (MSDI and Autofac) you are able to use scoped filters. Those filters will be resolved with current container scope, if the scope is not available - new scope will be created using root container

## Available Context Types

| Type                         | Usage                                                     |
| ---------------------------- | --------------------------------------------------------- |
| `ConsumeContext<T>`          | `UseConsumeFilter(typeof(TFilter<>), context)`            |
| `SendContext<T>`             | `UseSendFilter(typeof(TFilter<>), context)`               |
| `PublishContext<T>`          | `UsePublishFilter(typeof(TFilter<>), context)`            |
| `ExecuteContext<TArguments>` | `UseExecuteActivityFilter(typeof(TFilter<>), context)`    |
| `CompensateContext<TLog>`    | `UseCompensateActivityFilter(typeof(TFilter<>), context)` |

More information could be found inside [middleware](README.md) section

## Usage

::: warning
Methods with filter type should only be used for resolving filters with generic argument
:::

```csharp
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
        services.AddScoped(typeof(MySendFilter<>)); //register generic filter
          
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



