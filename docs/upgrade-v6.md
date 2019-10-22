# Upgrading to MassTransit v6

There have been several changes to v6 to reduce complexity, along with a few enhancements.


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

## Logging

The previous log abstraction used by MassTransit has been replaced with `Microsoft.Extensions.Logging.Abstractions`.

The previous log integration packages for Log4Net, NLog, and Serilog have been deprecated. An `ILoggerFactory` instance can be
configured for MassTransit by calling:

```csharp
LogContext.ConfigureCurrentLogContext(loggerFactory);
```

This should be done prior to configuring the bus.

### DiagnosticSource

As of version 6, MassTransit now uses DiagnosticSource for tracking messaging operations, such as Send, Receive, Publish, Consume, etc. An `Activity` is
created for each operation, and context-relevant tags and baggage are added.

To use a specific DiagnosticSource with MassTransit, you can specify it as an additional argument when configuring the LogContext.

```csharp
LogContext.ConfigureCurrentLogContext(loggerFactory, diagnosticListener);
```

> If you are using Application Insights (with Azure), you can connect a listener and all message activity will be reported automatically.

## Receive Endpoint Configuration

When MassTransit underwent a major overhaul, and multiple host support was added, that seemed like a great idea. A single bus talking to more than one broker, doing messaging. *Reality* &emdot; nobody used it. It added a lot of complexity, that wasn't used. 

With version 6, a single bus has a single host. That's it. Simple. And with this change, it's now no longer necessary to specify the host when configuring a receive endpoint. Yes, the old methods are there, and a pseudo-host is returned from the `.Host()` call which can still be passed, but it is ignored. All the transport-specific configuration methods are still there, without the `host` parameter.

So, enjoy the simplicity. Under the covers some other things were also made simple &emdot; but I doubt you'll notice.

## Courier

To be consistent with the rest of MassTransit, many of the interfaces in Courier has been renamed. For example, `ExecuteActivity<TArguments>` is now
`IExecuteActivity<TArguments>`. The previous interfaces are still supported, but have been marked obsolete. 

## Conductor

Hard things are hard. Building distributed applications at scale is a hard thing, and it's hard. In fact, it is really hard.

_Conductor wants to make it easier, with less complexity._

## MassTransit Host Service

Previous version of MassTransit provided a generalized service host, built using Topshelf, to get started with your first project. But the world has changed. With ASP.NET Core 3.0, and all the goodness that is the generic host, the developer community has moved to a new place.

The latest version of the host is built entirely from scratch, and takes advantage of:

- Microsoft Dependency Injection
- .NET Core Generic Host
- Microsoft Extensions Logging
- MassTransit's most excellent container registration/discovery extensions

Once it's built, the details will all be... revealed!

