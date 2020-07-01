# Configuration

MassTransit can be configured in most .NET application types. Several commonly used application types are documented below, including the package references used, and show the minimal configuration required. More thorough configuration details can be found throughout the documentation.

::: tip Containers
Unless the application type requires a dependency injection container, the examples below do not include examples of using one. To learn about container configuration, including examples, refer to the [containers](/usage/containers) section.
:::

The configuration examples all use the `EventContracts.ValueEntered` message type. The message type is only included in the first example's source code.

## Console App

> Uses [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/)

A console application, such as an application created using `dotnet new console`, has a `Main` entry point in the `Program.cs` class by default. In this example, MassTransit is configured to connect to RabbitMQ (which should be accessible on _localhost_) and publish messages. As each value is entered, the value is published as a `ValueEntered` message. No consumers are configured in this example.

<<< @/docs/code/configuration/ConsoleAppPublisher.cs

Another console application can be created to consume the published events. In this application, the receive endpoint is configured with a consumer that consumes the `ValueEntered` event. The message contract from the example above, in the same namespace, should be copied to this program as well (it isn't shown below).

<<< @/docs/code/configuration/ConsoleAppListener.cs

## ASP.NET Core

> Uses [MassTransit.AspNetCore](https://nuget.org/packages/MassTransit.AspNetCore/), [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/) 

MassTransit fully integrates with ASP.NET Core, including:

 * Microsoft Extensions Dependency Injection container configuration, including consumer, saga, and activity registration. The MassTransit interfaces are also registered:
   * _IBusControl_ (singleton)
   * _IBus_ (singleton)
   * _ISendEndpointProvider_ (scoped)
   * _IPublishEndpoint_ (scoped)
 * The _MassTransitHostedService_ to automatically start and stop the bus
 * Health checks for the bus and receive endpoints
 * Configures the bus to use _ILoggerFactory_ from the container

To produce messages from an ASP.NET Core application, the configuration below is used.

<<< @/docs/code/configuration/AspNetCorePublisher.cs

In this example, MassTransit is configured to connect to RabbitMQ (which should be accessible on localhost) and publish messages. The messages can be published from a controller as shown below.

<<< @/docs/code/configuration/AspNetCorePublisherController.cs

An ASP.NET Core application can also configure receive endpoints. The consumer, along with the receive endpoint, is configured within the _AddMassTransit_ configuration. Separate registration of the consumer is not required (and discouraged), however, any consumer dependencies should be added to the container separately. Consumers are registered as scoped, and dependencies should be registered as scoped when possible, unless they are singletons.

<<< @/docs/code/configuration/AspNetCoreListener.cs

MassTransit includes an endpoint name formatter (_IEndpointNameFormatter_) which can be used to automatically format endpoint names based upon the consumer, saga, or activity name. Using the _ConfigureEndpoints_ method will automatically create a receive endpoint for every added consumer, saga, and activity. To automatically configure the receive endpoints, use the updated configuration shown below.

The example sets the kebab-case endpoint name formatter, which will create a receive endpoint named `value-entered-event` for the `ValueEnteredEventConsumer`. The _Consumer_ suffix is removed by default. The endpoint is named based upon the consumer name, not the message type, since a consumer may consume multiple message types yet still be configured on a single receive endpoint.

<<< @/docs/code/configuration/AspNetCoreEndpointListener.cs

To configure health checks, which MassTransit will produce when using the _MassTransitHostedService_, add the health checks to the container and map the readiness and liveness endpoints. The following example also separates the readiness from the liveness health check.

<<< @/docs/code/configuration/AspNetCorePublisherHealthCheck.cs

## Topshelf Service

> Uses [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/), [Topshelf](https://www.nuget.org/packages/Topshelf/)

MassTransit recommends running consumers, sagas, and activities in an autonomous (standalone) service. Services can be managed by the operating system, and monitored using application performance monitoring tools. With .NET Core, this can be any application built using the .NET Core Generic Host. However, when using the .NET Framework (the full framework, such as .NET 4.6.1), a Windows service may be built using Topshelf.

::: tip Topshelf or .NET Core Generic Host
Before the .NET Core Generic Host became available, Topshelf was recommended to create a Windows service. Now that the Generic Host has built-in support for Windows services and Linux daemons, Topshelf is no longer required.
:::

The example below configures a receive endpoint, which is created and started with the service.

<<< @/docs/code/configuration/TopshelfListener.cs

