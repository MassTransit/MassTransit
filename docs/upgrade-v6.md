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

## Courier

To be consistent with the rest of MassTransit, many of the interfaces in Courier has been renamed. For example, `ExecuteActivity<TArguments>` is now
`IExecuteActivity<TArguments>`. The previous interfaces are still supported, but have been marked obsolete. 



