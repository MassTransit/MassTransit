# Containers

MassTransit supports several popular dependency injection containers. However, MassTransit does not require a container, so don't just reach for one when getting started. If you are already using a container in your application, see if your container is supported and reference the appropriate NuGet package.

MassTransit has a consistent API for registration and configuration when used with a container, and also includes conventions for configuring receive endpoints for the registered consumers, sagas, and routing slip activities. Under the hood, each container is configured to properly interact with MassTransit, leveraging available container features, such as nested scopes for message consumers, without requiring the developer to explictly configure every consumer.

::: tip NOTE
Dependency Injection styles are a personal choice that each developer or organization must make on their own. We recognize this choice, and respect it, and will not judge those who don't use a particular container or style of dependency injection. In short, we care.
:::

## Registration

MassTransit supports registration of consumers, sagas, and Courier activities for Autofac, Castle Windsor, Lamar, Microsoft Extensions Dependency Injection, Simple Injector, and StructureMap. When using one of these containers, the following registration methods should be used.

The `.AddMassTransit` extension method, which is specific to each container, supports both registration and bus configuration.

```cs
containerBuilder.AddMassTransit(r =>
{
    // register consumers, sagas, and Courier activities

    // register the bus in the container
    r.AddBus(context => Bus.Factory.CreateUsingInMemory(cfg =>
    {
        // configure receive endpoints, etc.
    }));
});
```

### Consumer Registration

To add a consumer registration, there are various methods available.

```cs
containerBuilder.AddMassTransit(r =>
{
    // add a consumer
    r.AddConsumer<UpdateCustomerAddressConsumer>();

    // add a consumer by type
    r.AddConsumer(typeof(UpdateCustomerAddressConsumer));

    // add a consumer by type, including a consumer definition for that consumer
    r.AddConsumer(typeof(UpdateCustomerAddressConsumer), typeof(UpdateCustomerAddressConsumerDefinition))

    // add all consumers in the specified assembly
    r.AddConsumers(Assembly.GetExecutingAssembly());

    // add multiple consumers and/or consumer definitions by type
    r.AddConsumers(typeof(ConsumerOne), typeof(ConsumerTwo));

    // adds ConsumerOne and its definition, and also adds ConsumerTwo
    r.AddConsumers(typeof(ConsumerOne), typeof(ConsumerOneDefinition), typeof(ConsumerTwo));

    // add consumers from the namespace containing the type
    r.AddConsumersFromNamespaceContaining<UpdateCustomerAddressConsumer>();
    r.AddConsumersFromNamespaceContaining(typeof(UpdateCustomerAddressConsumer));
});
```

To add a consumer registration and configure the consumer endpoint in the same expression, a definition can automatically be created.

```cs
containerBuilder.AddMassTransit(r =>
{
    r.AddConsumer<UpdateCustomerAddressConsumer>()
        .Endpoint(e =>
        {
            // customize the endpoint name
            e.Name = "customer-update";

            // specify the endpoint as temporary (may be non-durable, auto-delete, etc.)
            e.Temporary = false;

            // specify an optional concurrent message limit for the consumer
            e.ConcurrentMessageLimit = 8;

            // only use if needed, a sensible default is provided, and a reasonable
            // value is automatically calculated based upon ConcurrentMessageLimit if 
            // the transport supports it.
            e.PrefetchCount = 16;
        });
});
```

> The endpoint configuration is available for all registration types, including consumers, sagas and Courier activities.



## Endpoint Configuration

When configuring the bus, using `.AddBus`, recieve endpoints can be explicitly configured, or configured using the registered consumers, sagas, and Courier activities along with an _endpoint name formatter_.

```cs
containerBuilder.AddMassTransit(r =>
{
    // register consumers, sagas, and Courier activities

    // register the bus in the container
    r.AddBus(context => Bus.Factory.CreateUsingInMemory(cfg =>
    {
        // configure endpoints for all registered consumer, saga, and Courier activities
        cfg.ConfigureEndpoints(context);
    }));
});
```

There are three endpoint name formatters included:

- Default, which trims the _Consumer_, _Saga_, or _Activity_ extension from the class name and uses it for the queue
- SnakeCase, which trims and inserts an underscore before each uppercase letter
- KababCase, which trims and inserts a hyphen before each uppercase letter

The default requires no additional configuration, whereas the other two can either be registered in the container or specified on the `ConfigureEndpoints` call.

```cs
containerBuilder.AddMassTransit(r =>
{
    r.AddBus(context => Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.ConfigureEndpoints(context, KebabCaseEndpointNameFormatter.Instance);
    }));
});
```

### Saga Registration

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

> Endpoint configuration is available for all registration types, including consumers, sagas and Courier activities. Endpoint configuration can also be specifed in the definition, and overridden if specified in the .Add/.Endpoint methods.

**Hey! Where is my container??**

Containers come and go, so if you don't see your container here, or feel that the support for you container is weaksauce, pull requests are always welcome. Using an existing container it should be straight forward to add support for your favorite ÜberContainer.

* [Autofac](autofac)
* [Ninject](ninject)
* [StructureMap](structuremap)
* [Lamar](lamar)
* [Unity](unity)
* [Castle Windsor](castlewindsor)
* [Microsoft Dependency Injection](msdi)
