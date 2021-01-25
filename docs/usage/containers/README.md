# Containers

MassTransit supports several dependency injection containers. And since Microsoft introduced its own container, it has become the most commonly used container.

::: tip Optional
MassTransit does not require a container, as demonstrated in the [configuration example](/usage/configuration). So if you aren't already using a container, you can get started without having adopt one. However, when you're ready to use a container, perhaps to deploy your service using the .NET Generic Host, you will likely want to use Microsoft's built-in solution.
:::

Regardless of which container is used, supported containers have a consistent registration syntax used to add consumers, sagas, and activities, as well as configure the bus. Behind the scenes, MassTransit is configuring the container, including container-specific features such as scoped lifecycles, consistently and correctly. Use of the registration syntax has drastically reduced container configuration support questions.

## Consumer Registration

> Uses [MassTransit.Extensions.DependencyInjection](https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection/)

To configure a bus using RabbitMQ and register the consumers, sagas, and activities to be used by the bus, call the `AddMassTransit` extension method. The _UsingRabbitMq_ method can be changed to the appropriate method for the proper transport if RabbitMQ is not being used.

<<< @/docs/code/containers/MicrosoftContainer.cs

The `AddConsumer` method is one of several methods used to register consumers, some of which are shown below.

<<< @/docs/code/containers/MicrosoftContainerAddConsumer.cs

## Consumer Definition

A consumer definition is used to configure the receive endpoint and pipeline behavior for the consumer. When scanning assemblies or namespaces for consumers, consumer definitions are also found and added to the container. The _SubmitOrderConsumer_ and matching definition are shown below.

<<< @/docs/code/containers/ContainerConsumers.cs

## Endpoint Definition

To configure the endpoint for a consumer registration, or override the endpoint configuration in the definition, the `Endpoint` method can be added to the consumer registration. This will create an endpoint definition for the consumer, and register it in the container. This method is available on consumer and saga registrations, with separate execute and compensate endpoint methods for activities.

<<< @/docs/code/containers/MicrosoftContainerAddConsumerEndpoint.cs

When the endpoint is configured after the _AddConsumer_ method, the configuration overrides then endpoint configuration in the consumer definition. However, it cannot override the `EndpointName` if it is specified in the constructor. The order of precedence for endpoint naming is explained below.

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


## Bus Configuration

In the above examples, the bus is configured by the _UsingRabbitMq_ method, which is passed two arguments. `context` is the registration context, used to configure endpoints. `cfg` is the bus factory configurator, used to configure the bus. The above examples use the default conventions to configure the endpoints. Alternatively, endpoints can be explicitly configured. However, when configuring endpoints manually, the _ConfigureEndpoints_ methods should not be used (duplicate endpoints may result).

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

## Saga Registration

To add a state machine saga, use the _AddSagaStateMachine_ methods. For a consumer saga, use the _AddSaga_ methods.

::: tip Important
State machine sagas should be added before class-based sagas, and the class-based saga methods should not be used to add state machine sagas. This may be simplified in the future, but for now, be aware of this registration requirement.
:::

```cs
containerBuilder.AddMassTransit(r =>
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
containerBuilder.AddMassTransit(r =>
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
