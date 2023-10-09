# Logging

The MassTransit framework has fully adopted the [`Microsoft.Extensions.Logging`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0) framework.
So, it will use whatever logging configuration is already in your container.

## Configuration

By integrating with `Microsoft.Extensions.Logging` the basic configuration is no configuration. :tada:
When you run a project using the HostBuilder features of .Net you will get a basic logging experience right
out of the box.

## Serilog

At MassTransit, we are big fans of [Serilog](https://serilog.net/) and use this default configuration as a starting point in
most projects.

```sh
dotnet add package Serilog.Extensions.Hosting
dotnet add package Serilog
dotnet add package Serilog.Sinks.Console
```

```csharp
public static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .UseSerilog((host, log) =>
        {
            if (host.HostingEnvironment.IsProduction())
                log.MinimumLevel.Information();
            else
                log.MinimumLevel.Debug();

            log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
            log.MinimumLevel.Override("Quartz", LogEventLevel.Information);
            log.WriteTo.Console();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}
```

## Other Loggers

For applications that are not using MassTransit's container-based configuration (`AddMassTransit`) or for those with non-standard log configurations, it's possible to explicitly configure MassTransit so that it uses a provided `ILoggerFactory`.


```csharp
ILoggerFactory loggerFactory = <get this somehow>;

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    LogContext.ConfigureCurrentLogContext(loggerFactory);
});
```

This _must_ be specified within the bus configuration so that the provided `ILoggerFactory` is used.
