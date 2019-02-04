# Quickstart

In your ASP.NET Core Startup.cs file add the following

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // other config...
    
    services.AddSignalR().AddMassTransitBackplane(typeof(Startup).Assembly); // This is the first important line

    // Other config perhaps...

    // creating the bus config
    services.AddMassTransit(x =>
    {
        // Add this for each Hub you have
        x.AddSignalRHubConsumers<ChatHub>();

        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            var host = cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            // Register endpoint for each hub you have
            cfg.AddSignalRHubEndpoints<ChatHub, IRabbitMqReceiveEndpointConfigurator>(provider, host, e =>
            {
                e.AutoDelete = true;
                e.Durable = false;
            });
            
            // Or for azure Service Bus
            // Register endpoint for each hub you have
            cfg.AddSignalRHubEndpoints<ChatHub, IServiceBusReceiveEndpointConfigurator>(provider, host, e =>
            {
                e.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                e.RemoveSubscriptions = true;
            });
        }));
    });
}
```

There you have it. All the consumers needed for the backplane are added to a temporary endpoint. ReceiveEndpoints without any queue name are considered Non Durable, and Auto Deleting.