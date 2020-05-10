# Configuration

MassTransit can be used in any .NET application, however, the application type can influence the bus configuration. Several different application type examples are shown below, each of which lists any additional dependencies are required. Each example focuses on simplicity, and therefore may omit certain extra features to avoid confusion. 

::: tip NOTE
Except where required by the application type, no dependency injection container is used. For container configuration examples, refer to the [containers](/usage/containers) section.
:::

## Console application

A console application has a `Main` entry point, which is part of the `Program.cs` class by default. The example below configures a simple bus instance that publishes an event with a value entered.

> References: [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/)

```cs
namespace EventPublisher
{
    using MassTransit;

    public interface ValueEntered
    {
        string Value { get; }
    }

    public class Program
    {
        // be sure to set the C# language version to 7.3 or later
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg => cfg.Host("localhost"));

            // Important! The bus must be started before using it!
            await busControl.StartAsync();
            try
            {
                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    await busControl.Publish<ValueEntered>(new
                    {
                        Value = value
                    });
                }
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
```

In the example, the bus is configured and started after which a publishing loop allows values to be entered and published. When the loop exits, the bus is stopped.

::: warning REMEMBER
Always start the bus before using it.
:::

## ASP.NET Core

MassTransit integrates natively with ASP.NET Core, and supports:

 * Hosted service to start and stop the bus following the application lifecycle
 * Registers the bus instance as a singleton for the required interfaces
 * Adds health checks for the bus instance and receive endpoints
 * Configures the bus to use the host process `ILoggerFactory` instance

If you want to register your consumers in the ASP.NET Core service collection, use the following code as an example:

> References: [MassTransit.AspNetCore](https://nuget.org/packages/MassTransit.AspNetCore/), [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/) 

```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddMvc();
        
        // Consumer dependencies should be scoped
        services.AddScoped<SomeConsumerDependency>()

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderConsumer>();

            x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                // configure health checks for this bus instance
                cfg.UseHealthCheck(context);

                cfg.Host("rabbitmq://localhost");

                cfg.ReceiveEndpoint("submit-order", ep =>
                {
                    ep.PrefetchCount = 16;
                    ep.UseMessageRetry(r => r.Interval(2, 100));

                    ep.ConfigureConsumer<OrderConsumer>(context);
                });
            }));
        });

        services.AddMassTransitHostedService();
    }
}
```

Remember, however, that it is perfectly fine to set up all the required dependencies in the `Startup`, which serves as the bootstrap code for your application. By doing that you can avoid weird cases when you have two dependencies that implement the same interface and then you cannot properly register them both, since only the last one will count.

To make the health checks work, remember to add this line to the `Configure` method in the `Startup.cs` file. The `endpoints.MapControllers()` call is included by default, the `MapHealthChecks` is the only addition required.

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();

        // The readiness check uses all registered checks with the 'ready' tag.
        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
        {
            Predicate = (check) => check.Tags.Contains("ready"),
        });

        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
        {
            // Exclude all checks and return a 200-Ok.
            Predicate = (_) => false
        });
    });
}
```

To enable a separate readiness check from the health check, the services can be configured to separate the two.

```cs
 services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(2);
    options.Predicate = (check) => check.Tags.Contains("ready");
});
```

Also, as an obvious requirement, you need to use .NET Core 2.2 (or higher) and have those two package references in your project file:

```xml
<PackageReference Include="Microsoft.AspNetCore.Diagnostics" Version="2.2.0" />
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.HealthChecks" Version="2.2.0" />
```

Of course, health checks only work when you provide an endpoint that is accessible via http or https. That implies that you cannot do that using the `Microsoft.NET.Sdk`, so you need to ensure that your `csproj` file starts with this line:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
```

The Web SDK is used by default if you are using the ASP.NET Core Web Application template, but you can also change it, if you used the Console Application template. In such a case you'd also need to add some more packages to be able to host the http endpoint using Kestrel. The easiest way to migrate a console application to the web application (that still runs as a console application) by creating a new web application with the type `Empty` and checking the differences.

## Windows service

A Windows service is recommended for consuming commands and events as it provides an autonomous execution environment for message consumers. The service can be started and stopped using the service control manager, as well as monitored by operations tools.

::: tip
To create a Windows service, we strongly recommend using Topshelf, as it was built specifically for this purpose. Topshelf is easy to use, has zero dependencies, and creates a service that can be self-installed without additional tools.
:::

The important aspect of configuring a bus in a Windows service is to ensure that the bus is only configured and started when the service is *started*.

```csharp
namespace EventService
{
    using MassTransit;
    using Topshelf;

    public class Program
    {
        public static int Main()
        {
            return (int)HostFactory.Run(cfg => cfg.Service(x => new EventConsumerService()));
        }
    }

    class EventConsumerService :
        ServiceControl
    {
        IBusControl _bus;

        public bool Start(HostControl hostControl)
        {
            _bus = ConfigureBus();
            _bus.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _bus?.Stop(TimeSpan.FromSeconds(30));

            return true;
        }

        IBusControl ConfigureBus()
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");

                cfg.ReceiveEndpoint("event_queue", e =>
                {
                    e.Handler<ValueEntered>(context =>
                        Console.Out.WriteLineAsync($"Value was entered: {context.Message.Value}"));
                })
            });
        }
    }
}
```

## Web application

Configuring a bus in a web site is typically done to publish events, send commands, as well as engage in request/response conversations. Hosting receive endpoints and persistent consumers is not recommended (use a service as shown above).

### ASP.NET (MVC/WebApi2)

In a web application, the `HttpApplication` class methods of `Application_Start` and `Application_End` are used to configure/start the bus and stop the bus respectively.

> While many MassTransit samples use Topshelf, web applications are an exception where the standard web application conventions are followed.

```csharp
public class MvcApplication : HttpApplication
{
    static IBusControl _busControl;

    public static IBus Bus
    {
        get { return _busControl; }
    }

    protected void Application_Start()
    {
        _busControl = ConfigureBus();
        _busControl.Start();
    }

    protected void Application_End()
    {
        _busControl.Stop(TimeSpan.FromSeconds(10));;
    }

    IBusControl ConfigureBus()
    {
        return Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("localhost");
        });
    }
}

public class NotifyController : Controller
{
    public async Task<ActionResult> Put(string value)
    {
        await MvcApplication.Bus.Publish<ValueNotified>(new
        {
            Value = value
        });

        return View();
    }
}

public class CommandController : Controller
{
    public async Task<ActionResult> Send(string value)
    {
        var endpoint = await MvcApplication.Bus.GetSendEndpoint(_serviceAddress);

        await endpoint.Send<SubmitValue>(new
        {
            Timestamp = DateTime.UtcNow,
            Value = value
        });

        return View();
    }
}
```

The above example is kept simple, providing a static `MvcApplication.Bus` property to access the bus instance (for publishing events, and sending commands to endpoints). Newer version of ASP.NET have built-in dependency resolution, in which case the `IBus` should be registered so that controllers can specify the dependency in the constructor. In fact, the inherited `IPublishEndpoint` and `ISendEndpointProvider` should also be registered.

The example controllers show how to publish and send messages as well.

### OWIN Pipeline (WebApi2)

WebApi2 can alternatively use the OWIN pipeline. But OWIN doesn't have `Application_End` and so it requires a different approach to ensure the bus is disposed properly.

This example shows an OWIN WebApi2 project using the Autofac Container. The concept should be similar if you aren't using WebApi2 or a different Container, but still OWIN.

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        // STANDARD WEB API SETUP:

        // Get your HttpConfiguration. In OWIN, you'll create one
        // rather than using GlobalConfiguration.
        var config = new HttpConfiguration();

        WebApiConfig.Register(config); // Register your routes

        // Make the autofac container
        var builder = new ContainerBuilder();

        // Register your Bus
        builder.Register(c => Bus.Factory.CreateUsingRabbitMq(sbc => sbc.Host("localhost","dev")))
            .As<IBusControl>()
            .As<IBus>()
            .As<IPublishEndpoint>()
            .As<ISendEndpointProvider>()
            .SingleInstance();

        // Register anything else you might need...

        // Build the container
        var container = builder.Build();

        // OWIN WEB API SETUP:

        // set the DependencyResolver
        config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        // Register the Autofac middleware FIRST, then the Autofac Web API middleware,
        // and finally the standard Web API middleware.

        app.UseAutofacMiddleware(container);
        app.UseAutofacWebApi(config);
        app.UseWebApi(config);

        // Starts Mass Transit Service bus, and registers stopping of bus on app dispose
        var bus = container.Resolve<IBusControl>();
        var busHandle = bus.Start();

        var properties = new AppProperties(app.Properties);

        if(properties.OnAppDisposing != CancellationToken.None)
        {
            properties.OnAppDisposing.Register(() => busHandle.Stop(TimeSpan.FromSeconds(30)));
        }
    }
}
```

The important bit is the last several lines where we resolve the `IBusControl` and register an action to stop when the app disposes.

You'll also notice we registered the bus `.As<IPublishEndpoint>()`. That's because we are following the guidance above, where our websites should really only be Publishing to the service bus.

Now your controller might look like:

```csharp
public class MyController : ApiController
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MyController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IHttpActionResult> OrderShipped(int orderId)
    {
        await _publishEndpoint.Publish<OrderShipped>(
        new
        {
            OrderId = orderId
        });

        return Ok("Success!");
    }
}
```
