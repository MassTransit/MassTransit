# Hangfire Scheduler

### Configuring Hangfire Scheduler

::: warning
MassTransit will create own Hangfire Server which will be only listening to its related jobs.
:::

By default MassTransit is using static Hangfire configuration

```csharp
//your hangfire configuration
//NOTE: you need to configure hangfire before bus started

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.UseHangfireScheduler("hangfire", options => { /*configure background server*/ });
});
```

### Configuring the Hangfire address    

The bus has an internal context that is used to make it so that consumers that need to schedule 
messages do not have to be aware of the specific scheduler type being used, or the message scheduler 
address. To configure the address, use the extension method shown below.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/hangfire"));
});
```

Once configured, messages may be scheduled.

### Advance configuration

If you are using Hangfire integration with ASP.NET Core or non static configuration you can provide your configuration for MassTransit implementing `IHangfireComponentResolver`. For example using `IServiceProvider`:

```csharp
public class ServiceProviderHangfireComponentResolver : 
    IHangfireComponentResolver
{
    readonly IServiceProvider _serviceProvider;

    public ServiceProviderHangfireComponentResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBackgroundJobClient BackgroundJobClient => _serviceProvider.GetService<IBackgroundJobClient>();
    public IRecurringJobManager RecurringJobManager => _serviceProvider.GetService<IRecurringJobManager>();
    public ITimeZoneResolver TimeZoneResolver => _serviceProvider.GetService<ITimeZoneResolver>();
    public IJobFilterProvider JobFilterProvider => _serviceProvider.GetService<IJobFilterProvider>();
    public IEnumerable<IBackgroundProcess> BackgroundProcesses => _serviceProvider.GetServices<IBackgroundProcess>();
    public JobStorage JobStorage => _serviceProvider.GetService<JobStorage>();
}

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
  	
    cfg.UseHangfireScheduler(resolver, "hangfire", options => 
    { 
      	/*configure background server*/
    });
});
```
