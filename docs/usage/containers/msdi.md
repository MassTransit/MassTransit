# Configuring Microsoft Dependency Injection

**Important**
This documentation applies to ASP.NET Core 2.0 and 2.1. For the ASP.NET Core 2.2, you can use the 
[ASP.NET Core integration](../configuration.md#masstransit-and-aspnet-core) package that does many things out of the box.

MassTransit supports the default dependency injection framework used by ASP.NET.

> Support requires an additional NuGet package, `MassTransit.Extensions.DependencyInjection`, which is available using [NuGet](https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection/).

A sample project for the container registration code is available on [GitHub](https://github.com/MassTransit/Sample-Containers).

A working example is available on GitHub as well, visit the [repository](https://github.com/phatboyg/Sample-DotNetCore-DI/).

To configure the bus for use by ASP.NET, modify the `Startup.cs` file as shown.

```csharp
using MassTransit.ExtensionsDependencyInjectionIntegration;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderConsumer>();

            x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host("localhost", "/", h => { });

                cfg.ReceiveEndpoint(host, "submit-order", e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, 100));

                    e.ConfigureConsumer<OrderConsumer>(provider);
                    
                    EndpointConvention.Map<SubmitOrder>(e.InputAddress);
                });

                // or, configure the endpoints by convention
                cfg.ConfigureEndpoints(provider);
            }));

            x.AddRequestClient<SubmitOrder>();
        }));

        services.AddSingleton<IHostedService, BusService>();
    }
}
```

Once the bus is configured, a hosted service is used to start/stop the bus with the application. It is registered in the configuration above, using the interface type `IHostedService`.

```csharp
public class BusService :
    IHostedService
{
    private readonly IBusControl _busControl;

    public BusService(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _busControl.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _busControl.StopAsync(cancellationToken);
    }
}
```

Also configured in the above example is the use of the request client, to send a request from a controller.

```csharp
[Route("/orders")]
public class OrderController :
    Controller
{
    private readonly IRequestClient<SubmitOrder> _requestClient;

    public Default(IRequestClient<SubmitOrder> requestClient)
    {
        _requestClient = requestClient;
    }

    [HttpPost]
    public async Task<IActionResult> Submit(OrderModel model, CancellationToken cancellationToken)
    {
        try
        {
            var request = _requestClient.Create(new {OrderId = ...}, cancellationToken);

            var response = await request.GetResponse<OrderAccepted>();

            return Content($"Order Accepted: {response.Message.SomeValue}");
        }
        catch (RequestTimeoutException exception)
        {
            return StatusCode((int) HttpStatusCode.RequestTimeout);
        }
    }
}
```
