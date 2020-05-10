# Configuration

To configure service endpoints, change the `ConfigureEndpoints` method to `ConfigureServiceEndpoints`. To use the service client, add the `AddServiceClient` method.

```cs {14-18,21}
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SubmitOrderConsumer>();
            x.AddConsumer<AuthorizeOrderConsumer>()

            x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");

                var serviceInstanceOptions = new ServiceInstanceOptions()
                    .EnableInstanceEndpoint()
                    .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

                cfg.ConfigureServiceEndpoints(context, serviceInstanceOptions);
            }));

            x.AddServiceClient();
            x.AddRequestClient<SubmitOrder>();
            x.AddRequestClient<AuthorizeOrder>();
        });

        services.AddSingleton<IHostedService, BusService>();
    }
}
```

For real, that's it. The service is now _Conductor_ enabled.