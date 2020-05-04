# Quickstart

In your ASP.NET Core Startup.cs file add the following

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // other config...
    
    services.AddSignalR();

    // Other config perhaps...

    // creating the bus config
    services.AddMassTransit(x =>
    {
        // Add this for each Hub you have
        x.AddSignalRHub<ChatHub>(cfg => {/*Configure hub lifetime manager*/});

        x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            var host = cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
          
            // register consumer' and hub' endpoints
            cfg.ConfigureEndpoints(context);
        }));
    });

    services.AddMassTransitHostedService();
}
```

There you have it. All the consumers needed for the backplane are added to a temporary endpoint. ReceiveEndpoints without any queue name are considered Non Durable, and Auto Deleting.
