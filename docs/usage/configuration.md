# Configuration

MassTransit can be used in most .NET application types. Commonly used application types are documented below, including the package references used, and show the minimal configuration required. More thorough configuration details can be found throughout the documentation.

> The configuration examples all use the `EventContracts.ValueEntered` message type. The message type is only included in the first example's source code.

## Configuration

> Uses [MassTransit](https://nuget.org/packages/MassTransit/), [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/) 

MassTransit is easily configured in ASP.NET Core or .NET Generic Host applications (using .NET 3.1 or later). The built-in configuration will:

 * Add several interfaces (and their implementations)
   * _IBusControl_ (singleton)
   * _IBus_ (singleton)
   * _IReceiveEndpointConnector_ (singleton)
   * _ISendEndpointProvider_ (scoped)
   * _IPublishEndpoint_ (scoped)
 * Add a hosted service to start and stop the bus (or buses)
 * Add health checks for the bus and receive endpoints
 * Use `ILoggerFactory` to create log writers

To configure MassTransit so that it can be used to send/publish messages, the configuration below is recommended as a starting point.

<<< @/docs/code/configuration/AspNetCorePublisher.cs

In this example, MassTransit is configured to connect to RabbitMQ (which should be accessible on localhost) and publish messages. The messages can be published from a controller as shown below.

<<< @/docs/code/configuration/AspNetCorePublisherController.cs

## Consumers

To configure a bus using RabbitMQ and register the consumers, sagas, and activities to be used by the bus, call the `AddMassTransit` extension method. The _UsingRabbitMq_ method can be changed to the appropriate method for the proper transport if RabbitMQ is not being used.

<<< @/docs/code/containers/MicrosoftContainer.cs

The `AddConsumer` method is one of several methods used to register consumers, some of which are shown below.

<<< @/docs/code/containers/MicrosoftContainerAddConsumer.cs

### Consumer Definition

A consumer definition is used to configure the receive endpoint and pipeline behavior for the consumer. When scanning assemblies or namespaces for consumers, consumer definitions are also found and added to the container. The _SubmitOrderConsumer_ and matching definition are shown below.

<<< @/docs/code/containers/ContainerConsumers.cs

### Endpoint Definition

To configure the endpoint for a consumer registration, or override the endpoint configuration in the definition, the `Endpoint` method can be added to the consumer registration. This will create an endpoint definition for the consumer, and register it in the container. This method is available on consumer and saga registrations, with separate execute and compensate endpoint methods for activities.

<<< @/docs/code/containers/MicrosoftContainerAddConsumerEndpoint.cs

When the endpoint is configured after the _AddConsumer_ method, the configuration then overrides the endpoint configuration in the consumer definition. However, it cannot override the `EndpointName` if it is specified in the constructor. The order of precedence for endpoint naming is explained below.

1. Specifying `EndpointName = "submit-order-extreme"` in the constructor which cannot be overridden

    ```cs
    x.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>()

    public SubmitOrderConsumerDefinition()
    {
        EndpointName = "submit-order-extreme";
    }
    ```

2. Specifying `.Endpoint(x => x.Name = "submit-order-extreme")` in the consumer registration, chained to `AddConsumer`

    ```cs
    x.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>()
        .Endpoint(x => x.Name = "submit-order-extreme");

    public SubmitOrderConsumerDefinition()
    {
        Endpoint(x => x.Name = "not used");
    }
    ```

3. Specifying `Endpoint(x => x.Name = "submit-order-extreme")` in the constructor, which creates an endpoint definition

    ```cs
    x.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>()

    public SubmitOrderConsumerDefinition()
    {
        Endpoint(x => x.Name = "submit-order-extreme");
    }
    ```

4. Unspecified, the endpoint name formatter is used (in this case, the endpoint name is `SubmitOrder` using the default formatter)

    ```cs
    x.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>()

    public SubmitOrderConsumerDefinition()
    {
    }
    ```


### Saga Registration

To add a state machine saga, use the _AddSagaStateMachine_ methods. For a consumer saga, use the _AddSaga_ methods.

::: tip Important
State machine sagas should be added before class-based sagas, and the class-based saga methods should not be used to add state machine sagas. This may be simplified in the future, but for now, be aware of this registration requirement.
:::

```cs
services.AddMassTransit(r =>
{
    // add a state machine saga, with the in-memory repository
    r.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .InMemoryRepository();

    // add a consumer saga with the in-memory repository
    r.AddSaga<OrderSaga>()
        .InMemoryRepository();

    // add a saga by type, without a repository. The repository should be registered
    // in the container elsewhere
    r.AddSaga(typeof(OrderSaga));

    // add a state machine saga by type, including a saga definition for that saga
    r.AddSagaStateMachine(typeof(OrderState), typeof(OrderStateDefinition))

    // add all saga state machines by type
    r.AddSagaStateMachines(Assembly.GetExecutingAssembly());

    // add all sagas in the specified assembly
    r.AddSagas(Assembly.GetExecutingAssembly());

    // add sagas from the namespace containing the type
    r.AddSagasFromNamespaceContaining<OrderSaga>();
    r.AddSagasFromNamespaceContaining(typeof(OrderSaga));
});
```

To add a saga registration and configure the consumer endpoint in the same expression, a definition can automatically be created.

```cs
services.AddMassTransit(r =>
{
    r.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .NHibernateRepository()
        .Endpoint(e =>
        {
            e.Name = "order-state";
            e.ConcurrentMessageLimit = 8;
        });
});
```

Supported saga persistence storage engines are documented in the [saga documentation](/usage/sagas/persistence) section.

## Receive Endpoints

In the above examples, the bus is configured by the _UsingRabbitMq_ method, which is passed two arguments. `context` is the registration context, used to configure endpoints. `cfg` is the bus factory configurator, used to configure the bus. The above examples use the default conventions to configure the endpoints. Alternatively, endpoints can be explicitly configured. However, when configuring endpoints manually, the _ConfigureEndpoints_ methods may be excluded otherwise it should appear **after** any manually configured receive endpoints.

_ConfigureEndpoints_ uses an `IEndpointNameFormatter` to generate endpoint names, which by default uses a _PascalCase_ formatter. There are two additional endpoint name formatters included, snake and kebab case.

For the _SubmitOrderConsumer_, the endpoint names would be:

| Formatter | Name
|:---|:---
| Default | `SubmitOrder`
| Snake Case | `submit_order`
| Kebab Case | `submit-order`

All of the included formatters trim the _Consumer_, _Saga_, or _Activity_ suffix from the end of the class name. If the consumer name is generic, the generic parameter type is used instead of the generic type.

::: tip Video
Learn about the default conventions as well as how to tailor the naming style to meet your requirements in [this short video](https://youtu.be/bsUlQ93j2MY).
:::

The endpoint name formatter can be set as shown below.

<<< @/docs/code/containers/MicrosoftContainerFormatter.cs

The endpoint formatter can also be passed to the _ConfigureEndpoints_ method as shown.

<<< @/docs/code/containers/MicrosoftContainerFormatterInline.cs

To explicitly configure endpoints, use the _ConfigureConsumer_ and/or _ConfigureConsumers_ methods.

<<< @/docs/code/containers/MicrosoftContainerConfigureConsumer.cs

When using _ConfigureConsumer_, the _EndpointName_, _PrefetchCount_, and _Temporary_ properties of the consumer definition are not used.

MassTransit includes an endpoint name formatter (_IEndpointNameFormatter_) which can be used to automatically format endpoint names based upon the consumer, saga, or activity name. Using the _ConfigureEndpoints_ method will automatically create a receive endpoint for every added consumer, saga, and activity. To automatically configure the receive endpoints, use the updated configuration shown below.

The example sets the kebab-case endpoint name formatter, which will create a receive endpoint named `value-entered-event` for the `ValueEnteredEventConsumer`. The _Consumer_ suffix is removed by default. The endpoint is named based upon the consumer name, not the message type, since a consumer may consume multiple message types yet still be configured on a single receive endpoint.

<<< @/docs/code/configuration/AspNetCoreEndpointListener.cs

An ASP.NET Core application can also configure receive endpoints. The consumer, along with the receive endpoint, is configured within the _AddMassTransit_ configuration. Separate registration of the consumer is not required (and discouraged), however, any consumer dependencies should be added to the container separately. Consumers are registered as scoped, and dependencies should be registered as scoped when possible, unless they are singletons.

<<< @/docs/code/configuration/AspNetCoreListener.cs

To configure health checks, which MassTransit will produce when using the _MassTransitHostedService_, add the health checks to the container and map the readiness and liveness endpoints. The following example also separates the readiness from the liveness health check.

<<< @/docs/code/configuration/AspNetCorePublisherHealthCheck.cs

::: tip Optional
MassTransit does not require a container. If you aren't already using a container, you can get started without having adopt one. However, when you're ready to use a container, perhaps to deploy your service using the .NET Generic Host, MassTransit is ready with fully integrated support for Microsoft.Extensions.DependencyInjection.
:::

Regardless of which container is used, supported containers have a consistent registration syntax used to add consumers, sagas, and activities, as well as configure the bus. Behind the scenes, MassTransit is configuring the container, including container-specific features such as scoped lifecycles, consistently and correctly. Use of the registration syntax has drastically reduced container configuration support questions.


## .NET Generic Host

> Uses [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/)

The .NET Generic Host is the preferred way to create standalone services and can be easily using `dotnet new worker`. In this example, MassTransit is configured to connect to RabbitMQ (which should be accessible on _localhost_) and publish messages. As each value is entered, the value is published as a `ValueEntered` message. No consumers are configured in this example.

<<< @/docs/code/configuration/ConsoleAppPublisher.cs

Another console application can be created to consume the published events. In this application, the receive endpoint is configured with a consumer that consumes the `ValueEntered` event. The message contract from the example above, in the same namespace, should be copied to this program as well (it isn't shown below).

<<< @/docs/code/configuration/ConsoleAppListener.cs

