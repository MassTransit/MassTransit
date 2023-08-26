# Logging

The MassTransit framework has fully adopted the [`Microsoft.Extensions.Logging`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0) framework.
So, it will use whatever logging configuration is already in your container.

## Basic Configuration

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
