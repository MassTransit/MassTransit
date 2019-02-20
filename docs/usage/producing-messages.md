# Producing messages
<!-- TOC -->

- [Sending commands](#sending-commands)
    - [Using endpoint conventions](#using-endpoint-conventions)
    - [Sending via interfaces](#sending-via-interfaces)
    - [Setting message headers](#setting-message-headers)
- [Publishing events](#publishing-events)

<!-- /TOC -->
An application or service can produce messages using two different methods. A message can be sent using `Send` or a message can be published using `Publish`. The behavior of each method is very different, but it easy to understand by looking at the type of messages involved with each particular method.

When a message is sent, it is delivered to a specific endpoint using a `DestinationAddress`. When a message is published, it is not sent to a specific endpoint, but is instead broadcasted to any consumers which have *subscribed* to the message type. For these two separate behavior, we describe messages sent as commands, and messages published as events.

> These are discussed in depth in the *Creating a message contract* section of the documentation.

## Sending commands

Sending a command to an endpoint requires an `ISendEndpoint` reference, which can be obtained from any send endpoint provider (an object that supports `ISendEndpointProvider`). The application should always use the object closest to it to obtain the send endpoint, and it should do it every time it needs it -- the application should not cache the send endpoint reference.

For instance, an `IBus` instance is a send endpoint provider, but it should *never* be used by a consumer to obtain an `ISendEndpoint`. `ConsumeContext` can also provide send endpoints, and should be used since it is *closer* to the consumer.

<div class="alert alert-info">
<b>Note:</b>
	This cannot be stressed enough -- always get send endpoints from the closest interface to the application code. There is extensive logic to tie message flows together using conversation, correlation, and initiator identifiers. By skipping a level and going outside the closest scope, that information can be lost which prevents the useful trace identifiers from being properly handled.
</div>

To obtain a send endpoint from a send endpoint provider, use the GetSendEndpoint() method as shown below. Once the send endpoint is returned, it can be used to a send a message.

```csharp
public async Task SendOrder(ISendEndpointProvider sendEndpointProvider)
{
    var endpoint = await sendEndpointProvider.GetSendEndpoint(_serviceAddress);

    await endpoint.Send(new SubmitOrder(...));
}
```

There are many overloads for the `Send` method. Because MassTransit is built around filters and pipes, pipes are used to customize the message delivery behavior of Send. There are also some useful overloads (via extension methods) to make sending easier and less noisy due to the pipe construction, etc.

### Using endpoint conventions

Using send endpoints might seem too verbose, because before sending any message, you need to get the send endpoint and to do that you need to have an endpoint address. Usually, addresses are kept in the configuration and accessing the configuration from all over the application is not a good practice.

Endpoint conventions solve this issue by allowing you to configure the mapping between message types and endpoint addresses. A potential downside here that you will not be able to send messages of the same type to different endpoints by using conventions. If you need to do this, keep using the `GetSendEndpoint` method.

Conventions are configured like this:

```csharp
EndpointConvention.Map<SubmitOrder>(
    new Uri("rabbitmq://mq.acme.com/order/order_processing"));
```

Now, you don't need to get the send endpoint anymore for this type of message and can send it like this:

```csharp
public async Task Post(SubmitOrderRequest request)
{
    if (AllGoodWith(request))
        await _bus.Send(ConvertToCommand(request));
}
```

Also, from inside the consumer, you can do the same using the `ConsumeContext.Send` overload:

```csharp
EndpointConvention.Map<StartDelivery>(
    new Uri(ConfigurationManager.AppSettings["deliveryServiceQueue"]));
```

```csharp
public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    private readonly IOrderSubmitter _orderSubmitter;

    public SubmitOrderConsumer(IOrderSubmitter submitter)
        => _orderSubmitter = submitter;

    public async Task Consume(IConsumeContext<SubmitOrder> context)
    {
        await _orderSubmitter.Process(context.Message);

        await context.Send(
            new StartDelivery(context.Message.OrderId, DateTime.UtcNow));
    }
}
```

The `EndpointConvention.Map<T>` method is static, so it can be called from everywhere. It is important to remember that you cannot configure conventions for the same message twice. If you try to do this - the `Map` method will throw an exception. This is also important when writing tests, so you need to configure the conventions at the same time as you confgigure your test bus (harness).

It is better to configure send conventions before you start the bus.

### Sending via interfaces

Since the general recommendation is to use interfaces, there are convenience methods to initialize the interface without requiring the creation of a message class underneath. While versioning of messages still requires a class which supports multiple interfaces, a simple approach to send an interface message is shown below.

```csharp
public interface SubmitOrder
{
    string OrderId { get; }
    DateTime OrderDate { get; }
    decimal OrderAmount { get; }
}

public async Task SendOrder(ISendEndpoint endpoint)
{
    await endpoint.Send<SubmitOrder>(new
    {
        OrderId = "27",
        OrderDate = DateTime.UtcNow,
        OrderAmount = 123.45m
    });
}
```

### Setting message headers

There are a variety of message headers available which are used for correlation and tracking of messages. It is also possible to override some of the default behaviors of MassTransit when a fault occurs. For instance, a fault is normally *published* when a consumer throws an exception. If instead the application wants faults delivered to a specific address, the ``FaultAddress`` can be specified via a header. How this is done is shown below.

```csharp
public interface SubmitOrder
{
    string OrderId { get; }
    DateTime OrderDate { get; }
    decimal OrderAmount { get; }
}

public async Task SendOrder(ISendEndpoint endpoint)
{
    await endpoint.Send<SubmitOrder>(new
    {
        OrderId = "27",
        OrderDate = DateTime.UtcNow,
        OrderAmount = 123.45m
    }, context => 
        context.FaultAddress = new Uri("rabbitmq://localhost/order_faults"));
}
```

## Publishing events

Messages are published similarly to how messages are sent, but in this case, a single `IPublishEndpoint` is used. The same rules for endpoints apply, the closest instance of the publish endpoint should be used. So the `ConsumeContext` for consumers, and `IBus` for applications that are published outside of a consumer context.

To publish a message, see the code below:

```csharp
public interface OrderSubmitted
{
    string OrderId { get; }
    DateTime OrderDate { get; }
}

public async Task NotifyOrderSubmitted(IPublishEndpoint publishEndpoint)
{
    await publishEndpoint.Publish<OrderSubmitted>(new
    {
        OrderId = "27",
        OrderDate = DateTime.UtcNow,
    });
}
```

If you are planning to publish messages from within your consumers, this example would suit better:

```csharp
public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    private readonly IOrderSubmitter _orderSubmitter;

    public SubmitOrderConsumer(IOrderSubmitter submitter)
        => _orderSubmitter = submitter;

    public async Task Consume(IConsumeContext<SubmitOrder> context)
    {
        await _orderSubmitter.Process(context.Message);

        await context.Publish<OrderSubmitted>(new
        {
            OrderId = context.Message.OrderId,
            OrderDate = DateTime.UtcNow
        })
    }
}

```
