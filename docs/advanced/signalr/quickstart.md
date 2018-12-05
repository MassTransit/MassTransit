# Quickstart

In your ASP.NET Core Startup.cs file add the following

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // other config...
    services.AddMassTransitBackplane(out var hubConsumers, typeof(Startup).Assembly); // This is the first important line

    // Other config perhaps...

    // creating the bus config
    services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        // other config...

        // Creates a non-durable, temporary endpoint per hub
        cfg.CreateBackplaneEndpoints<IRabbitMqReceiveEndpointConfigurator>(provider, host, hubConsumers, e=>
        {
            e.AutoDelete = true;
            e.Durable = false;
        });
    }));
}
```

There you have it. All the consumers needed for the backplane are added to a temporary endpoint. ReceiveEndpoints without any queue name are considered Non Durable, and Auto Deleting.