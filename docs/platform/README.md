# Platform

MassTransit supports building, deploying, and monitoring services on a container-based platform. The platform provides a consistent hosting environment for consumers, sagas, and activities, eliminating duplicated service code (no more cut-and-pasting `Program.cs`). The platform Docker images can be used to deploy services to any container-based environment.

The platform image is hosted on [Docker](https://hub.docker.com/r/masstransit/platform) and is updated independent of the MassTransit package.

There are transport images as well, including [RabbitMQ](https://hub.docker.com/r/masstransit/rabbitmq) and [ActiveMQ](https://hub.docker.com/r/masstransit/active).

A preconfigured image for scheduling messages using [Quartz](https://hub.docker.com/r/masstransit/quartz) is also available. The [source](https://github.com/MassTransit/Platform-Quartz) is a good example of how to build an assembly for hosting on the platform. A preconfigured [SQL Server](https://hub.docker.com/r/masstransit/sqlserver-quartz) container is also available for development purposes.

These images can be [configured](/platform/configuration) to specify the transport, as well as other options.

There is also a [Live-Coding Video](https://www.youtube.com/watch?v=-xEnO9H62lk) showing the Twitch sample being converted to run on the platform.

To build a service using the MassTransit Platform, create a startup class that implemented the `IPlatformStartup` interface.

> Package: [MassTransit.Platform.Abstractions](https://nuget.org/packages/MassTransit.Platform.Abstractions)

```cs
public class OrderServicePlatformStartup :
    IPlatformStartup
{
    readonly ILogger<OrderServicePlatformStartup> _logger;

    public QuartzPlatformStartup(IConfiguration configuration, ILogger<OrderServicePlatformStartup> logger)
    {
        _logger = logger;
    }

    public void ConfigureMassTransit(IServiceCollectionConfigurator configurator, IServiceCollection services)
    {
        _logger.LogInformation("Configuring Order Service");

        configurator.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));
    }

    public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IServiceProvider provider)
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
    }
}
```

The example adds a consumer, including a consumer definition. When the bus is started, an endpoint will be created by convention for the consumer.

### Adding Configuration

To access configuration options, such as `appsettings.json` or environment variables, add the configuration classes in the platform startup class.

```cs
public class OrderServicePlatformStartup :
    IPlatformStartup
{
    readonly IConfiguration _configuration;

    public QuartzPlatformStartup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureMassTransit(IServiceCollectionConfigurator configurator, IServiceCollection services)
    {
        services.Configure<OrderServiceOptions>(_configuration.GetSection("OrderService"));
    }
}
```