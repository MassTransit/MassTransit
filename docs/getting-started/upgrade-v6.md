# Upgrading

There have been several changes to v6 to reduce complexity, along with a few enhancements.

::: tip NOTE
MassTransit requires either .NET Standard 2.0 or .NET Framework 4.6.1 (or later). .NET 4.5.2 is no longer supported.
:::

## Automatonymous

In previous version, using Automatonymous required an additional package, `MassTransit.Automatonymous`. The contents of that package are now
included in the `MassTransit` assembly/package, which now depends on `Automatonymous`. This was done to reduce the number of extra packages
required for container support (along with state machine registration), as well as improve the saga repository persistence assemblies.

When upgrading to v6, any references to the old `MassTransit.Automatonymous` package should be removed.

If you are using a container with MassTransit, and were using one of the old container packages for Automatonymous, those package references
should also be removed. With version 6, only the single container integration package is required (such as `MassTransit.Autofac` or
`MassTransit.Extensions.DependencyInjection`).

The following packages are available for the supported containers:

- MassTransit.Autofac
- MassTransit.Extensions.DependencyInjection
- MassTransit.Lamar
- MassTransit.Ninject (no registration support)
- MassTransit.SimpleInjector
- MassTransit.StructureMap
- MassTransit.Unity (no registration support)
- MassTransit.Windsor

### Saga Repository Update (v6.1+)

The saga repositories have been completely refactored, to eliminate duplicate logic and increase consistency across the various storage engines. All repositories also now support the container registration extensions, which provides a consistent syntax for registering and configuring saga repositories for use with dependency injection containers. When using the `.AddMassTransit()` container registration method, a repository can now be registered with the saga. For details, see the updated [documentation](/usage/sagas/persistence).

### Azure Service Bus

The previous (now legacy) **MassTransit.AzureServiceBus** package, which was only maintained to continue support for .NET 4.5.2, has been deprecated. Going forward, the **MassTransit.Azure.ServiceBus.Core** package should be used. The package supports both .NET 4.6.1 and .NET Standard 2.0. With the new package, the .NET Messaging protocol is no longer supported. The new package includes both AMQP and WebSocket support. Certain corporate firewall configurations that previously used .NET Messaging instead of AMQP may need to specify the web socket protocol to connect to Azure Service Bus.

### Logging

The previous log abstraction used by MassTransit has been replaced with `Microsoft.Extensions.Logging.Abstractions`.

The previous log integration packages for Log4Net, NLog, and Serilog have been deprecated. An `ILoggerFactory` instance can be
configured for MassTransit by calling:

```csharp
LogContext.ConfigureCurrentLogContext(loggerFactory);
```

This should be done prior to configuring the bus. 

::: tip
If you are using the new `.AddMassTransit()` configuration, combined with `.AddBus()`, then _ILoggerFactory_ is automatically configured for you. In this case, the statement above is not required.
:::

### DiagnosticSource

As of version 6, MassTransit now uses DiagnosticSource for tracking messaging operations, such as Send, Receive, Publish, Consume, etc. An `Activity` is
created for each operation, and context-relevant tags and baggage are added.

MassTransit follows the [guidance](https://github.com/dotnet/runtime/blob/master/src/libraries/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md) from Microsoft. To connect listeners, look at the [section](https://github.com/dotnet/runtime/blob/master/src/libraries/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md#subscribe-to-diagnosticsource) that explains how to connect.

### Receive Endpoint Configuration

When MassTransit underwent a major overhaul, and multiple host support was added, that seemed like a great idea. A single bus talking to more than one broker, doing messaging. *Reality* &emdot; nobody used it. It added a lot of complexity, that wasn't used. 

With version 6, a single bus has a single host. That's it. Simple. And with this change, it is no longer necessary to specify the host when configuring a receive endpoint. Yes, the old methods are there, and a pseudo-host is returned from the `.Host()` call which can still be passed, but it is ignored. All the transport-specific configuration methods are still there, without the `host` parameter.

So, enjoy the simplicity. Under the covers some other things were also made simple &emdot; but I doubt you'll notice.

### Courier

To be consistent with the rest of MassTransit, many of the interfaces in Courier has been renamed. For example, `ExecuteActivity<TArguments>` is now
`IExecuteActivity<TArguments>`. The previous interfaces are still supported, but have been marked obsolete. 

### Conductor (coming soon)

Hard things are hard. Building distributed applications at scale is a hard thing, and it's hard. In fact, it is really hard.

So hard that it isn't ready yet - but there is enough other stuff to warrant releasing v6 without it.

_Conductor wants to make it easier, with less complexity._


### MassTransit Platform

Previous version of MassTransit provided a generalized service host, built using Topshelf, to get started with your first project. But the world has changed. With ASP.NET Core 3.1, and all the goodness that is the generic host, the developer community has moved to a new place.

MassTransit.Host is being replaced with the new [Platform](/platform), which is a Docker-based solution for consistent service deployment using MassTransit.
