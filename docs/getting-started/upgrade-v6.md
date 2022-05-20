# Upgrading


## Version 8

MassTransit v8 is the first major release since the availability of .NET 6. MassTransit v8 works a significant portion of the underlying components into a more manageable solution structure. Focused on the developer experience, while maintaining compatibility with previous versions, this release brings together the entire MassTransit stack.

Automatonymous, Green Pipes, and NewId have been completely integrated into a single MassTransit solution. This means that every aspect of MassTransit is now within a single namespace, which makes it easy to find the right interface, extension, and whatever else is needed. A lot of common questions result in a missing `using` statement, and now that should no longer be the case. The entire developer surface area, for the most part, exists within the `MassTransit` namespace.

When upgrading from previous versions of MassTransit, there are a few initial steps to get up and running. While this list doesn't cover everything, these are the main items experienced so far when upgrading from a previous version.

- Remove any references to packages that were not updated with v8. This includes:
  - `GreenPipes`
  - `NewId`  (still available separately, do not use in a project referencing MassTransit)
  - `Automatonymous`
  - `Automatonymous.Visualizer` -> `MassTransit.StateMachineVisualizer`
  - `MassTransit.AspNetCore`
  - `MassTransit.Extensions.DependencyInjection`
  - Any of the third-party container assemblies.
- Remove any `using` statements that for namespaces that no longer exist

Some configuration interfaces have been removed/changed names:

| Original | New |
|--|--|
|`IServiceCollectionBusConfigurator`|`IBusRegistrationConfigurator`|
|`IServiceCollectionRiderConfigurator`|`IRiderRegistrationConfigurator`|
|`IServiceCollectionMediatorConfigurator`|`IMediatorRegistrationConfigurator`|

### Serialization

The default JSON serializer is now `System.Text.Json`. Refer to [Microsoft's Migration Guide](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to?pivots=dotnet-6-0) if you encounter any serialization issues after upgrading.

To continue using Newtonsoft for serialization, add the `MassTransit.Newtonsoft` package and specify one of the configuration methods when configuring the bus:
- `UseNewtonsoftJsonSerializer`
- `UseNewtonsoftRawJsonSerializer`
- `UseXmlSerializer`
- `UseBsonSerializer`

### Hosted Service

Previous versions of MassTransit required the use of the `MassTransit.AspNetCore` package to support registration of MassTransit's hosted service. This package is no longer required, and MassTransit will automatically add an `IHostedService` for MassTransit.

The host can be configured using `IOptions` configuration support, such as shown below:

```cs
services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
    options.StopTimeout = TimeSpan.FromMinutes(1);
});
```

> In addition to the hosted service, .NET health checks are added as well, and may be included on health check endpoints.

### Observers

Observers registered in the container will be connected to the bus automatically, including:

| Observer Type | Registration |
|--|--|
| `IBusObserver` | `AddBusObserver<T>` |
| `IReceiveObserver` | `AddReceiveObserver<T>` |
| `IConsumeObserver` | `AddConsumeObserver<T>` |
| `IReceiveEndpointObserver` | `AddReceiveEndpointObserver<T>` |

### State Machine Changes

The state machine interfaces, `BehaviorContext<T>` and `BehaviorContext<T, TData>` are now derived from `SagaConsumeContext<T>` and `SagaConsumeContext<T, TMessage>`. This significantly improves the usability of MassTransit features in state machine. No more calling `GetPayload<ConsumeContext>` or other methods to get access to the `ConsumeContext`! Seriously, this is awesome.

As part of this change, the `.Data` and `.Instance` properties of `BehaviorContext` are superfluous, and have subsequently been marked as obsolete. They still work, and return `.Message` or `.Saga` respectively, but eventually the might be removed (not in the near future though).

The previous `Automatonymous.Activity<T>` and `Automatonymous.Activity<T, TData>` interfaces have been renamed, and are now `IStateMachineActivity<TSaga>` and `IStateMachineActivity<TSaga, TMessage>` (both are now in the top-level `MassTransit` namespace).

Specifying headers when using the `.Init<T>()` message initializer with `SendAsync`, `PublishAsync`, and other related methods now works as expected!

The saga state machine test harness type `IStateMachineSagaTestHarness<TInstance, TStateMachine>` has been replaced with the properly named type `ISagaStateMachineTestHarness<TStateMachine, TInstance>`, which also has consistent generic argument ordering.

A new `.Retry()` activity has been added, allowing individual activities within a state machine to be retried. This retry is performed inline, with the same saga instance, and uses the same retry policies as message-based retry.

### Nullable Types

`MassTransit.Abstractions` has enabled nullable type information, so it may signal to the compiler that a null can be returned when appropriate for certain methods.

### Unit Testing

A new version of the test harness is now available, specifically designed for use with containers. The basics are the same, only the configuration has changed. An example test, shown below, using the in-memory transport by default. Consumer, saga, and activity test harnesses are added automatically and can be retrieved from the harness.

```cs
[Test]
public async Task The_consumer_should_respond_to_the_request()
{
    await using var provider = new ServiceCollection()
        .AddMassTransitTestHarness(x =>
        {
            x.AddConsumer<SubmitOrderConsumer>();
        })
        .BuildServiceProvider(true);

    var harness = provider.GetTestHarness();

    await harness.Start();

    var client = harness.GetRequestClient<SubmitOrder>();

    await client.GetResponse<OrderSubmitted>(new
    {
        OrderId = InVar.Id,
        OrderNumber = "123"
    });

    Assert.IsTrue(await harness.Sent.Any<OrderSubmitted>());

    Assert.IsTrue(await harness.Consumed.Any<SubmitOrder>());

    var consumerHarness = harness.GetConsumerHarness<SubmitOrderConsumer>();

    Assert.That(await consumerHarness.Consumed.Any<SubmitOrder>());
}
```

::: tip
When the _provider_ (which is an `IServiceProvider`) is disposed, it will dispose of the test harness, which will stop the bus.
:::

Additionally, the test harness can now be used with any transport. For example, to use RabbitMQ:

```cs
[Test]
public async Task Should_use_broker()
{
    await using var provider = new ServiceCollection()
        .AddMassTransitTestHarness(x =>
        {
            x.AddConsumer<SubmitOrderConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("some-broker-address", h =>
                {
                    h.Username("joe");
                    h.Password("cool");
                });

                cfg.ConfigureEndpoints(context);
            });
        })
        .BuildServiceProvider(true);
}
```

### Third-Party Container Support

MassTransit is now using _Microsoft.Extensions.DependencyInjection.Abstractions_ as an integral configuration component. This means that all configuration (such as `AddMassTransit`, `AddMediator`) is built against `IServiceCollection`. Support for other containers is provided using each specific container's extensions to work with `IServiceCollection` and `IServiceProvider`.

For example, using Autofac, the configuration might look something like what is shown below.

```cs
var collection = new ServiceCollection();

collection.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();

    x.UsingRabbitMq((context, cfg) => 
    {
        cfg.ConfigureEndpoints(context);
    });
});
var factory = new AutofacServiceProviderFactory();
var container = factory.CreateBuilder(collection);

return factory.CreateServiceProvider(container);
```

MassTransit would then be able to use `IServiceProvider` with Autofac to create scopes, resolve dependencies, etc.

### Transport Changes

- [RabbitMQ batch publishing](/usage/transports/rabbitmq.md#configurebatch) is now disabled by default. If you are seeing a degradation in publishing performance after upgrading, you may benefit from enabling batch publish to increase throughput. Scenarios where batching can improve throughput include high-latency broker connectivity and HA/Lazy queues.

## Version 7

As with previous major versions, MassTransit V7 includes a number of new features, but has also deprecated or changed from previous configuration syntax. For the most part, consumers, sagas, etc. should work exactly as they did with previous releases. However, some of the configuration aspects may have been updated to be more consistent.

### .NET Standard 2.0

MassTransit is now built for .NET Standard 2.0 only. The packages should be compatible with .NET Standard 2.0 (or later), as well as .NET Framework 4.6.1 (or later). Specific .NET Framework packages are no longer built or packaged.

### Riders

[Riders](/usage/riders/) are an entirely new feature which adds Kafka and Azure Event Hub support to MassTransit. A huge thanks to Denys Kozhevnikov [GitHub](https://github.com/MassTransit/MassTransit/commits?author=NooNameR) [@noonamer](https://twitter.com/noonamer) for his amazing effort!

### Configuration

Configuring MassTransit using a container has been streamlined. Refer to the [configuration](/usage/configuration) section for details. A brief summary:

-   The `.Host` methods are now void. The _IHost_ interfaces are no longer accessible (or needed).
-   `AddBus` has been superseded by `UsingRabbitMq` (and other transport-specific extension methods)

### Sagas

The SagaRepository standardization is now completed, and all other repositories have been removed (InMemorySagaRepository is still there though).

The send topology can now be used to configure the _CorrelationId_ that should be used, allowing state machine sagas to automatically configure events that correlate on a property that isn't implemented by `CorrelatedBy<Guid>`.

### Message Scheduler

A number of new container configuration options for configuring and registering the [message scheduler](/advanced/scheduling/scheduling-api) have been added. 

### Turnout

Turnout, which has been poorly supported since the beginning, has been rewritten from the ground up. Consumers can now use the `IJobConsumer<T>` interface to support long-running jobs managed by MassTransit. They are supported using Conductor, and a series of state machines to track job execution, retry, and concurrency. Check out the [job consumers](/advanced/job-consumers) section for details.

### Mediator

- Container configuration has changed, and now uses the `AddMediator` method (instead of `AddMassTransit`).
- Publish no longer throws if there are no consumers. To throw when publishing and no consumers are registered, set the _Mandatory_ flag on the _PublishContext_.
- Consumers can now be connected/detached after the mediator has been created.

### Testing

-   Test harnesses now use an inactivity timer to complete sooner once the bus stops processing messages.
-   Message lists, such as Consumed, Received, Sent, and Published, now have async _Any_ methods

### Transactions

The _transaction outbox_ has been renamed to _TransactionalBus_, to avoid confusion. See the [transactions section](/advanced/middleware/transactions) for details.

### Changed, Deprecated

The following packages have been deprecated and replaced with a new package:

* [MassTransit.DocumentDb](https://nuget.org/packages/MassTransit.DocumentDb/) <br/>Use [MassTransit.Azure.Cosmos](https://nuget.org/packages/MassTransit.Azure.Cosmos/) instead.
* [MassTransit.Lamar](https://nuget.org/packages/MassTransit.Lamar/) <br/> Use [MassTransit.Extensions.DependencyInjection](https://nuget.org/packages/MassTransit.Extensions.DependencyInjection/) to configure the container.
* [MassTransit.Host](https://nuget.org/packages/MassTransit.Host/) <br/>Use [MassTransit Platform](/platform) instead.

The following packages have been deprecated and are no longer supported:

* [MassTransit.Http](https://nuget.org/packages/MassTransit.Http/)
* [MassTransit.Ninject](https://nuget.org/packages/MassTransit.Ninject/)
* [MassTransit.Reactive](https://nuget.org/packages/MassTransit.Reactive/)
* [MassTransit.Unity](https://nuget.org/packages/MassTransit.Unity/)










## Version 6



### Automatonymous

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
- MassTransit.SimpleInjector
- MassTransit.StructureMap
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
