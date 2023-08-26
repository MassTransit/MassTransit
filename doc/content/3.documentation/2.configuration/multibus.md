# MultiBus

_pronounced mool-tee-buss_

MassTransit is designed so that most applications only need a single bus, and that is the recommended approach. Using a single bus, with however many receive endpoints are needed, minimizes complexity and ensures efficient broker resource utilization. Consistent with this guidance, container configuration using the `AddMassTransit` method registers the appropriate types so that they are available to other components, as well as consumers, sagas, and activities.

However, with broader use of cloud-based platforms comes a greater variety of messaging transports, not to mention HTTP as a transfer protocol. As application sophistication increases, connecting to multiple message transports and/or brokers is becoming more common. Therefore, rather than force developers to create their own solutions, MassTransit has the ability to configure additional bus instances within specific dependency injection containers.

> And by specific, right now it is very specific: Microsoft.Extensions.DependencyInjection. Though technically, any container that supports `IServiceCollection` for configuration _might_ work.

## Standard Configuration

To review, the configuration for a single bus is shown below.

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();
    x.AddRequestClient<SubmitOrder>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
```

This configures the container so that there is a bus, using RabbitMQ, with a single consumer `SubmitOrderConsumer`, using automatic endpoint configuration. The MassTransit hosted service, which configures the bus health checks and starts/stop the bus via `IHostedService`, is also added to the container.

When a consumer, a saga, or an activity is consuming a message the _ConsumeContext_ is available in the container scope. When the consumer is created using the container, the consumer and any dependencies are created within that scope. If a dependency includes _ISendEndpointProvider_, _IPublishEndpoint_, or even _ConsumeContext_ (should not be the first choice, but totally okay) on the constructor, all three of those interfaces result in the same reference which is great because it ensures that messages sent and/or published by the consumer or its dependencies includes the proper correlation identifiers and monitoring activity headers.

## MultiBus Configuration

To support multiple bus instances in a single container, the interface behaviors described above had to be considered carefully. There are expectations as to how these interfaces behave, and it was important to ensure consistent behavior whether an application has one, two, or a dozen bus instances (please, not a dozen – think of the children). A way to differentiate between different bus instances ensuring that sent or published messages end up on the right queues or topics is needed. The ability to configure each bus instance separately, yet leverage the power of a single shared container is also a must.

To configure additional bus instances, create a new interface that includes _IBus_. Then, using that interface, configure the additional bus using the `AddMassTransit<T>` method, which is included in the **_MassTransit.MultiBus_** namespace.

```csharp
public interface ISecondBus :
    IBus
{
}
```

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitOrderConsumer>();
    x.AddRequestClient<SubmitOrder>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

services.AddMassTransit<ISecondBus>(x =>
{
    x.AddConsumer<AllocateInventoryConsumer>();
    x.AddRequestClient<AllocateInventory>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("remote-host");

        cfg.ConfigureEndpoints(context);
    });
});
```

This configures the container so that there is an additional bus, using RabbitMQ, with a single consumer _AllocateInventoryConsumer_, using automatic endpoint configuration. Only a single hosted service is required that will start all bus instances so there is no need to add it twice.

Notable differences in the new method:

- The generic argument, _ISecondBus_, is the type that will be added to the container instead of _IBus_. This ensures that access to the additional bus is directly available without confusion.

## Using your MultiBus

For consumers or dependencies that need to send or publish messages to a different bus instance, a dependency on that specific bus interface (such as _IBus_, or _ISecondBus_) would be added.

::alert{type="warning"}
Some things do not work across bus instances. As stated above, calling Send or Publish on an IBus (or other bus instance interface) starts a new conversation. Middleware components such as the _InMemoryOutbox_ currently do not buffer messages across bus instances.
::

### Controller

```csharp
[ApiController]
[Route("/inventory")]
public class InventoryController : ControllerBase
{
    readonly Bind<ISecondBus, IPublishEndpoint> _publishEndpoint;

    public InventoryController(Bind<ISecondBus, IPublishEndpoint> publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Post() 
    {
        // .. do stuff
    }
}
```

### Razor Page

```csharp
public class InventoryPage : PageModel
{
    public void OnPost([FromServices] Bind<ISecondBus, IPublishEndpoint> publishEndpoint)
    {
        await publishEndpoint.Value.Publish<AllocateInventory>(new 
        {
            SomeData = { }
        })
    }
}
```

## Advanced Bus Types

In the example above, which should be the most common of this hopefully uncommon use, the _ISecondBus_ interface is all that is required. MassTransit creates a dynamic class to delegate the `IBus` methods to the bus instance. However, it is possible to specify a class that implements _ISecondBus_ instead.

To specify a class, as well as take advantage of the container to bring additional properties along with it, take a look at the following types and configuration.

```csharp
public interface IThirdBus :
    IBus
{
}

class ThirdBus :
    BusInstance<IThirdBus>,
    IThirdBus
{
    public ThirdBus(IBusControl busControl, ISomeService someService)
        : base(busControl)
    {
        SomeService = someService;
    }

    public ISomeService SomeService { get; }
}

public interface ISomeService
{
}
```

```csharp
services.AddMassTransit<IThirdBus>(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("third-host");
    });
});
```

This would add a third bus instance, the same as the second, but using the instance class specified. The class is resolved from the container and given `IBusControl`, which must be passed to the base class ensuring that it is properly configured.




## Container Registration Details

To support a first class experience with `Microsoft.Extensions.DependencyInjection` MassTransit registers common interfaces
for MultiBus instances using a `Bind<TKey, TValue>` that allows you to specify the owner of the type you are interested in. This
allows you to access various pieces of MassTransit outside of a Consumer. Below are two tables that list out the various items 
you might be interested in.

### First Bus

There are several interfaces added to the container using this configuration:

| Interface                     | Lifestyle | Notes                                                                  |
|:------------------------------|:----------|:-----------------------------------------------------------------------|
| `IBusControl`                 | Singleton | Used to start/stop the bus (not typically used)                        |
| `IBus`                        | Singleton | Publish/Send on this bus, starting a new conversation                  |
| `ISendEndpointProvider`       | Scoped    | Send messages from consumer dependencies, ASP.NET Controllers          |
| `IPublishEndpoint`            | Scoped    | Publish messages from consumer dependencies, ASP.NET Controllers       |
| `IClientFactory`              | Singleton | Used to create request clients (singleton, or within scoped consumers) |
| `IRequestClient<SubmitOrder>` | Scoped    | Used to send requests                                                  |
| `ConsumeContext`              | Scoped    | Available in any message scope, such as a consumer, saga, or activity  |

### Multibus

The registered interfaces are slightly different for additional bus instances.

| Interface                     | Lifestyle | Notes                                                                   |
|:------------------------------|:----------|:------------------------------------------------------------------------|
| `IBusControl`                 | N/A       | Not registered, but automatically started/stopped by the hosted service |
| ~~`IBus`~~                        | N/A       | Not registered, the new bus interface is registered instead             |
| `ISecondBus`                  | Singleton | Publish/Send on this bus, starting a new conversation                   |
| `ISendEndpointProvider`       | Scoped    | Send messages from consumer dependencies only                           |
| `IPublishEndpoint`            | Scoped    | Publish messages from consumer dependencies only                        |
| `IClientFactory`              | N/A       | Registered as an instance-specific client factory                       |
| `IRequestClient<SubmitOrder>` | Scoped    | Created using the specific bus instance                                 |
| `ConsumeContext`              | Scoped    | Available in any message scope, such as a consumer, saga, or activity   |
| `Bind<ISecondBus, ISendEndpointProvider>`  | Scoped    | Send messages from controllers or outside of a consumer context |
| `Bind<ISecondBus, IPublishEndpoint>`  | Scoped    | Publish messages from controllers or outside of a consumer context |
| `Bind<ISecondBus, IClientFactory>`  | Scoped    | Registered as an instance-specific client factory |
| `Bind<ISecondBus, IRequestClient<SubmitOrder>>`  | Scoped    | Created using the bound bus instance |
