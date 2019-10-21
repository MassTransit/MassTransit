# Observing messages

MassTransit supports a number of message observes, making it possible to monitoring when messages are received, consumed, sent, and published. Each type of observer is configured separately, keeping the interfaces lean and
focused.

<div class="alert alert-warning">
<b>Warning:</b>
    Observers should not be used to modify or intercept messages. To intercept messages (either to add headers,or modify the message contents), create a new or use an existing middleware component.
</div>

## Observing received messages

To observe received messages immediately after they are delivered by the transport, create a class that implements the `IReceiveObserver` interface, and connect it to the bus as shown below.

```csharp
public class ReceiveObserver : IReceiveObserver
{    
    public Task PreReceive(ReceiveContext context)
    {
        // called immediately after the message was delivery by the transport
    }

    public Task PostReceive(ReceiveContext context)
    {
        // called after the message has been received and processed
    }

    public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        where T : class
    {
        // called when the message was consumed, once for each consumer
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception) where T : class
    {
        // called when the message is consumed but the consumer throws an exception
    }

    public Task ReceiveFault(ReceiveContext context, Exception exception)
    {
        // called when an exception occurs early in the message processing, such as deserialization, etc.
    }
}
```

Then connect the observer to the bus before starting it, as shown.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
    {
        e.Consumer<UpdateCustomerConsumer>();
    });
});

var observer = new ReceiveObserver();
var handle = busControl.ConnectReceiveObserver(observer);
```

## Observing consumed messages

If the receive context isn't super interesting, perhaps the actual consumption of messages might float your boat. A consume observer implements the `IConsumeObserver` interface, as shown below.

```csharp
public class ConsumeObserver : IConsumeObserver
{    
    Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
    {
        // called before the consumer's Consume method is called
    }

    Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
    {
        // called after the consumer's Consume method is called
        // if an exception was thrown, the ConsumeFault method is called instead
    }

    Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
    {
        // called if the consumer's Consume method throws an exception
    }
}
```

To connect the observer, use the `ConnectConsumeObserver` method before starting the bus.

## Observing specific consumed messages

Okay, so it's obvious that if you've read this far you want a more specific observer, one that only is called when a specific message type is consumed. We have you covered there too, as shown below.

```csharp
public class ConsumeObserver<T> : IConsumeMessageObserver<T> where T : class
{
    Task IConsumeMessageObserver<T>.PreConsume(ConsumeContext<T> context)
    {
        // called before the consumer's Consume method is called
    }

    Task IConsumeMessageObserver<T>.PostConsume(ConsumeContext<T> context)
    {
        // called after the consumer's Consume method was called
        // again, exceptions call the Fault method.
    }

    Task IConsumeMessageObserver<T>.ConsumeFault(ConsumeContext<T> context, Exception exception)
    {
        // called when a consumer throws an exception consuming the message
    }
}
```

To connect the observer, use the `ConnectConsumeMessageObserver` method before starting the bus.

## Observing sent messages

Okay, so, incoming messages are not your thing. We get it, you're all about what goes out. It's cool. It's better to send than to receive. Or is that give? Anyway, a send observer is also available.

```csharp
public class SendObserver : ISendObserver
{
    public Task PreSend<T>(SendContext<T> context)
        where T : class
    {
        // called just before a message is sent, all the headers should be setup and everything
    }

    public Task PostSend<T>(SendContext<T> context)
        where T : class
    {
        // called just after a message it sent to the transport and acknowledged (RabbitMQ)
    }

    public Task SendFault<T>(SendContext<T> context, Exception exception)
        where T : class
    {
        // called if an exception occurred sending the message
    }
}
```

To connect the observer, you already guessed it, use the `ConnectSendObserver` method before starting the bus.

## Observing published messages

In addition to send, publish is also observable. Because the semantics matter, absolutely. Using the MessageId to link them up as it's unique for each message. Remember that Publish and Send are two distinct operations so if you want to observe all messages that are leaving your service, you have to connect both Publish and Send observers.

```csharp
public class PublishObserver : IPublishObserver
{
    public Task PrePublish<T>(PublishContext<T> context)
        where T : class
    {
        // called right before the message is published (sent to exchange or topic)
    }

    public Task PostPublish<T>(PublishContext<T> context)
        where T : class
    {
        // called after the message is published (and acked by the broker if RabbitMQ)
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception)
        where T : class
    {
        // called if there was an exception publishing the message
    }
}
```

Finally, to connect the observer, use the `ConnectPublishObserver` method before starting the bus.

These are a ton of interfaces, and they offer a lot of information about how the system is behaving under the hood. So use them, abuse them, bend them, and break them. Just realize, they are immediate, so don't be slow or your messaging will be equally slow.
