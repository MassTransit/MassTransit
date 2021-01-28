# Producers

An application or service can produce messages using two different methods. A message can be sent or a message can be published. The behavior of each method is very different, but it's easy to understand by looking at the type of messages involved with each particular method.

When a message is sent, it is delivered to a specific endpoint using a _DestinationAddress_. When a message is published, it is not sent to a specific endpoint, but is instead broadcasted to any consumers which have *subscribed* to the message type. For these two separate behavior, we describe messages sent as commands, and messages published as events.

> These are discussed in depth in the [Messages](messages) section of the documentation.

## Send

::: tip Video
Learn about `Send` in [this short video](https://youtu.be/t6FsmqZsdJk).
:::

To send a message, the _DestinationAddress_ is used to deliver the message to an endpoint — such as a queue. One of the `Send` method overloads on the `ISendEndpoint` interface is called, which will then send the message to the transport. An `ISendEndpoint` is obtained from one of the following objects:

1. The `ConsumeContext` of the message being consumed

   This ensures that the correlation headers, message headers, and trace information is propagated to the sent message.
2. An `ISendEndpointProvider` instance

   This may be passed as an argument, but is typically specified on the constructor of an object that is resolved using a dependency injection container.
3. The `IBus`

   The last resort, and should only be used for messages that are being sent by an _initiator_ — a process that is initiating a business process.

Once the `Send` method has been called (only once or repeatedly to send a series of messages), the `ISendEndpoint` reference should fall out of scope.

::: tip
Applications should not store the `ISendEndpoint` reference, it is automatically cached by MassTransit and discarded when it is no longer needed.
:::

For instance, an `IBus` instance is a send endpoint provider, but it should *never* be used by a consumer to obtain an `ISendEndpoint`. `ConsumeContext` can also provide send endpoints, and should be used since it is *closer* to the consumer.

::: warning
This cannot be stressed enough -- always obtain an `ISendEndpoint` from the closest scope. There is extensive logic to tie message flows together using conversation, correlation, and initiator identifiers. By skipping a level and going outside the closest scope, that critical information will be lost which prevents the useful trace identifiers from being propagated.
:::

### Send Endpoint

To obtain a send endpoint from a send endpoint provider, call the `GetSendEndpoint` method as shown below. The method is _async_, so be sure to _await_ the result.

```csharp
public class SubmitOrder
{
    public string OrderId { get; set; }
}

public async Task SendOrder(ISendEndpointProvider sendEndpointProvider)
{
    var endpoint = await sendEndpointProvider.GetSendEndpoint(_serviceAddress);

    await endpoint.Send(new SubmitOrder { OrderId = "123" });
}
```

There are many overloads for the `Send` method. Because MassTransit is built around filters and pipes, pipes are used to customize the message delivery behavior of Send. There are also some useful overloads (via extension methods) to make sending easier and less noisy due to the pipe construction, etc.

### Endpoint Address

An endpoint address is a fully-qualified URI which may include transport-specific details. For example, an endpoint on a local RabbitMQ server would be:

```
rabbitmq://localhost/input-queue
```

Transport-specific details may include query parameters, such as:

```
rabbitmq://localhost/input-queue?durable=false
```

This would configure the queue as non-durable, where messages would only be stored in memory and therefore would not survive a broker restart.

#### Short Addresses

Starting with MassTransit v6, short addresses are supported. For instance, to obtain a send endpoint for a queue on RabbitMQ, the caller would only have to specify:

```
GetSendEndpoint(new Uri("queue:input-queue"))
```

This would return a send endpoint for the _input-queue_ exchange, which would be bound to the _input-queue_ queue. Both the exchange and the queue would be created if either did not exist. This short syntax eliminates the need to know the scheme, host, port, and virtual host of the broker, only the queue and/or exchange details are required.

Each transport has a specific set of supported short addresses.


##### Supported Address Schemes

| Short Address | RabbitMQ           | Azure Service Bus  | ActiveMQ           | Amazon SQS         |
| ------------- |:------------------:|:------------------:|:------------------:|:------------------:|
| queue:name    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| topic:name    |                    | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| exchange:name | :heavy_check_mark: |                    |                    |                    |


### Address Conventions

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
EndpointConvention.Map<StartDelivery>(new Uri(ConfigurationManager.AppSettings["deliveryServiceQueue"]));
```

```csharp
public class SubmitOrderConsumer : 
    IConsumer<SubmitOrder>
{
    private readonly IOrderSubmitter _orderSubmitter;

    public SubmitOrderConsumer(IOrderSubmitter submitter)
        => _orderSubmitter = submitter;

    public async Task Consume(IConsumeContext<SubmitOrder> context)
    {
        await _orderSubmitter.Process(context.Message);

        await context.Send(new StartDelivery(context.Message.OrderId, DateTime.UtcNow));
    }
}
```

The `EndpointConvention.Map<T>` method is static, so it can be called from everywhere. It is important to remember that you cannot configure conventions for the same message twice. If you try to do this - the `Map` method will throw an exception. This is also important when writing tests, so you need to configure the conventions at the same time as you configure your test bus (harness).

It is better to configure send conventions before you start the bus.

## Publish

::: tip Video
Learn about `Publish` in [this short video](https://youtu.be/-MAEZq5G7lk).
:::

Messages are published similarly to how messages are sent, but in this case, a single `IPublishEndpoint` is used. The same rules for endpoints apply, the closest instance of the publish endpoint should be used. So the `ConsumeContext` for consumers, and `IBus` for applications that are published outside of a consumer context.

::: tip Key Concept
In MassTransit, _Publish_ follows the [publish subscribe][1] messaging pattern. For each message published, a copy of the message is delivered to each subscriber. The mechanism by which this happens is implemented by the message transport, but semantically the operation is the same regardless of which transport is used.
:::

The same guidelines apply for publishing messages, the closest object should be used.

1. The `ConsumeContext` of the message being consumed

   This ensures that the correlation headers, message headers, and trace information is propagated to the published message.
2. An `IPublishEndpoint` instance

   This may be passed as an argument, but is typically specified on the constructor of an object that is resolved using a dependency injection container.
3. The `IBus`

   The last resort, and should only be used for messages that are being published by an _initiator_ — a process that is initiating a business process.

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

## Message Initializers

MassTransit continues to encourage and support the use of interfaces for message contracts, and initializers make it easy to produce interface messages.

### Anonymous object values

`Send`, `Publish`, and most of the methods that behave in similar ways (scheduling, responding to requests, etc.) all support passing an object of _values_ which is used to set the properties on the specified interface. A simple example is shown below.

Consider this example message contract to submit an order.

```csharp
public interface SubmitOrder
{
    Guid OrderId { get; }
    DateTime OrderDate { get; }
    string OrderNumber { get; }
    decimal OrderAmount { get; }
}
```

To send this message to an endpoint:

```csharp
await endpoint.Send<SubmitOrder>(new
{
    OrderId = NewId.NextGuid(),
    OrderDate = DateTime.UtcNow,
    OrderNumber = "18001",
    OrderAmount = 123.45m
});
```

The anonymous object is loosely typed, the properties are matched by name, and there is an extensive set of type conversions that may occur to obtain match the types defined by the interface. Most numeric, string, and date/time conversions are supported, as well as several advanced conversions (including variables, and asynchronous `Task<T>` results).

Collections, including arrays, lists, and dictionaries, are broadly supported, including the conversion of list elements, as well as dictionary keys and values. For instance, a dictionary of (int,decimal) could be converted on the fly to (long, string) using the default format conversions.

Nested objects are also supported, for instance, if a property was of type `Address` and another anonymous object was created (or any type whose property names match the names of the properties on the message contract), those properties would be set on the message contract.

### Headers

Header values can be specified in the anonymous object using a double-underscore (pronounced 'dunder' apparently) property name. For instance, to set the message time-to-live, specify a property with the duration. Remember, any value that can be converted to a `TimeSpan` works!

```csharp
public interface GetOrderStatus
{
    Guid OrderId { get; }
}

var response = await requestClient.GetResponse<OrderStatus>(new 
{
    __TimeToLive = 15000, // 15 seconds, or in this case, 15000 milliseconds
    OrderId = orderId,
});
```

> actually, that's a bad example since the request client already sets the message expiration, but you, get, the, point.

To add a custom header value, a special property name format is used. In the name, underscores are converted to dashes, and double underscores are converted to underscores. In the following example:

```csharp
var response = await requestClient.GetResponse<OrderStatus>(new 
{
    __Header_X_B3_TraceId = zipkinTraceId,
    __Header_X_B3_SpanId = zipkinSpanId,
    OrderId = orderId,
});
```

This would include set the headers used by open tracing (or Zipkin, as shown above) as part of the request message so the service could share in the span/trace. In this case, `X-B3-TraceId` and `X-B3-SpanId` would be added to the message envelope, and depending upon the transport, copied to the transport headers as well.

### Variables

MassTransit also supports variables, which are special types added to the anonymous object. Following the example above, the initialization could be changed to use variables for the `OrderId` and `OrderDate`. Variables are consistent throughout the message creation, using the same variable multiple times returns the value. For instance, the Id created to set the _OrderId_ would be the same used to set the _OrderId_ in each item.

```csharp
public interface OrderItem
{
    Guid OrderId { get; }
    string ItemNumber { get; }
}

public interface SubmitOrder
{
    Guid OrderId { get; }
    DateTime OrderDate { get; }
    string OrderNumber { get; }
    decimal OrderAmount { get; }
    OrderItem[] OrderItems { get; }
}

await endpoint.Send<SubmitOrder>(new
{
    OrderId = InVar.Id,
    OrderDate = InVar.Timestamp,
    OrderNumber = "18001",
    OrderAmount = 123.45m,
    OrderItems = new[]
    {
        new { OrderId = InVar.Id, ItemNumber = "237" },
        new { OrderId = InVar.Id, ItemNumber = "762" }
    }
});
```

### Awaiting Task results

Message initializers are now asynchronous, which makes it possible to do some pretty cool things, including waiting for Task input properties to complete and use the result to initialize the property. An example is shown below.

```csharp
public interface OrderUpdated
{
    Guid CorrelationId { get; }
    DateTime Timestamp { get; }
    Guid OrderId { get; }
    Customer Customer { get; }
}

public async Task<CustomerInfo> LoadCustomer(Guid orderId)
{
    // work happens up in here
}

await context.Publish<OrderUpdated>(new
{
    InVar.CorrelationId,
    InVar.Timestamp,
    OrderId = context.Message.OrderId,
    Customer = LoadCustomer(context.Message.OrderId)
});
```

The property initializer will wait for the task result and then use it to initialize the property (converting all the types, etc. as it would any other object).

> While it is of course possible to await the call to `LoadCustomer`, properties are initialized in parallel, and thus, allowing the initializer to await the Task can result in better overall performance. Your mileage may vary, however.


## Send Headers

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
    }, context => context.FaultAddress = new Uri("rabbitmq://localhost/order_faults"));
}
```

Since a message initializer is being used, this can actually be simplified.

```csharp
public async Task SendOrder(ISendEndpoint endpoint)
{
    await endpoint.Send<SubmitOrder>(new
    {
        OrderId = "27",
        OrderDate = DateTime.UtcNow,
        OrderAmount = 123.45m,

        // header names are prefixed with __, and types are converted as needed
        __FaultAddress = "rabbitmq://localhost/order_faults"
    });
}
```


[1]: http://www.enterpriseintegrationpatterns.com/patterns/messaging/PublishSubscribeChannel.html
[2]: http://spring.io/blog/2011/04/01/routing-topologies-for-performance-and-scalability-with-rabbitmq/
[3]: http://codebetter.com/drusellers/2011/05/08/brain-dump-conventional-routing-in-rabbitmq/

