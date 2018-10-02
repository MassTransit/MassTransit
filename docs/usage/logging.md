# Logging

Logging in MassTransit is now done using an internal abstraction allowing you
to choose your preferred logging solution. MassTransit has a log named
`MassTransit.Messages` where all of the message traffic is logged.
This logging looks like:

```
RECV:{Address}:{Message Id}:{Message Type Name}
SEND:{Address}:{Message Name}
```

## Log4Net

To use log4net for MassTransit logging, you need to install the
`MassTransit.Log4Net` NuGet package.

Then add logging to your service bus initialization;

```csharp
using MassTransit.Log4NetIntegration;

XmlConfigurator.Configure();

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseLog4Net();
});
```

## NLog

To use NLog for MassTransit logging, you need to install the
`MassTransit.NLog` NuGet package.

Then add logging to your service bus initialization:

```csharp
using MassTransit.NLogIntegration;

//configure NLog

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    /* usual stuff */
    cfg.UseNLog();
});
```

## Serilog

To use Serilog for MassTransit logging, install the `MassTransit.SerilogIntegration`
package.

Then add logging to your service bus initialization:

```csharp
using MassTransit.SerilogIntegration;


//configure Serilog

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    /* usual stuff */

    // to use the global Log.Logger, use this:
    cfg.UseSerilog();

    // To use custom logger, use this:
    cfg.UseSerilog(logger);
});
```

## Microsoft Extensions Logger

To use Microsoft Extensions Logger for MassTransit logging, install the `MassTransit.Extensions.Logging`
package.

Then add logging to your service bus initialization:

```csharp

//configure ILoggerFactory

services.AddSingleton(sp => Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    /* usual stuff */

    // to use custom logger, use this:
    cfg.UseExtensionsLogging(new LoggerFactory());

    // To use configured logger, use this:
    cfg.UseExtensionsLogging(sp.GetRequiredService<ILoggerFactory>());
}));
```
