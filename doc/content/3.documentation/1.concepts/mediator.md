# Mediator

MassTransit includes a mediator implementation, with full support for consumers, handlers, and sagas (including saga state machines). MassTransit Mediator runs in-process and in-memory, no transport is required. For maximum performance, messages are passed by reference, instead than being serialized, and control flows directly from the _Publish_/_Send_ caller to the consumer. If a consumer throws an exception, the _Publish_/_Send_ method throws and the exception should be handled by the caller.

::alert{type="success"}
Mediator is a [behavioral design pattern](https://en.wikipedia.org/wiki/Mediator_pattern) in which a _mediator_ encapsulates communication between objects to reduce coupling.
::

## Configuration

To configure Mediator, use the _AddMediator_ method.

```csharp
services.AddMediator(cfg =>
{
    cfg.AddConsumer<SubmitOrderConsumer>();
    cfg.AddConsumer<OrderStatusConsumer>();
});
```

Consumers and sagas (including saga repositories) can be added, routing slip activities are not supported using mediator. Consumer and saga definitions are supported as well, but certain properties like _EndpointName_ are ignored. Middleware components, including _UseMessageRetry_ and _UseInMemoryOutbox_, are fully supported.

Once created, Mediator doesn't need to be started or stopped and can be used immediately. _IMediator_ combines several other interfaces into a single interface, including _IPublishEndpoint_, _ISendEndpoint_, and _IClientFactory_.

MassTransit dispatches the command to the consumer asynchronously. Once the _Consume_ method completes, the _Send_ method will complete. If the consumer throws an exception, it will be propagated back to the caller.

::callout
#summary
Send vs Publish
#content
_Send_ expects the message to be consumed. If there is no consumer configured for the message type, an exception will be thrown.

_Publish_, on the other hand, does not require the message to be consumed and does not throw an exception if the message isn't consumed. To throw an exception when a published message is not consumed, set the _Mandatory_ property to _true_ on _PublishContext_.
::

### Scoped Mediator

Main mediator interface `IMediator` is registered as a singleton but there is another scoped version of it `IScopedMediator`. This interface is registered as a part of current IoC scope (`HttpContext` or manually created) and can be used in order to share the scope for the entire pipeline.
By default with `IMediator`, each consumer has its own scope. By using `IScopedMediator`, the scope is shared between several consumers.

::alert{type="success"}
No additional configuration is required as long as Mediator is configured via `services.AddMediator()`
::

## Connect

Consumers can be connected and disconnected from mediator at run-time, allowing components and services to temporarily consume messages. Use the _ConnectConsumer_ method to connect a consumer. The handle can be used to disconnect the consumer.

```csharp
var handle = mediator.ConnectConsumer<SubmitOrderConsumer>();
```

## Requests

> TODO add example of using `Request<T>` in contract

To send a request using the mediator, a request client can be created from _IMediator_. The example below configures two consumers and then sends the _SubmitOrder_ command, followed by the _GetOrderStatus_ request.

```csharp
Guid orderId = NewId.NextGuid();

await mediator.Send<SubmitOrder>(new { OrderId = orderId });

var client = mediator.CreateRequestClient<GetOrderStatus>();

var response = await client.GetResponse<OrderStatus>(new { OrderId = orderId });
```

The _OrderStatusConsumer_, along with the message contracts, is shown below.

```csharp
public record GetOrderStatus
{
    public Guid OrderId { get; init; }
}

public record OrderStatus
{
    public Guid OrderId { get; init; }
    public string Status { get; init; }
}

class OrderStatusConsumer :
    IConsumer<GetOrderStatus>
{
    public async Task Consume(ConsumeContext<GetOrderStatus> context)
    {
        await context.RespondAsync<OrderStatus>(new
        {
            context.Message.OrderId,
            Status = "Pending"
        });
    }
}
```

Just like _Send_, the request is executed asynchronously. If an exception occurs, the exception will be propagated back to the caller. If the request times out, or if the request is canceled, the _GetResponse_ method will throw an exception (either a _RequestTimeoutException_ or an _OperationCanceledException_).

## Middleware

MassTransit Mediator is built using the same components used to create a bus, which means all the same middleware components can be configured. For instance, to configure the Mediator pipeline, such as adding a scoped filter, see the example below.

```csharp
public class ValidateOrderStatusFilter<T> :
    IFilter<SendContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        if (context.Message is GetOrderStatus getOrderStatus && getOrderStatus.OrderId == Guid.Empty)
            throw new ArgumentException("The OrderId must not be empty");

        return next.Send(context);
    }
}
```

```csharp
cfg.ConfigureMediator((context, mcfg) =>
{
    mcfg.UseSendFilter(typeof(ValidateOrderStatusFilter<>), context);
});
```
