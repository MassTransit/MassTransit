# Observers

MassTransit supports several message observers allowing received, consumed, sent, and published messages to be monitored. There is a bus observer as well, so that the bus life cycle can be monitored.

::: warning
Observers should not be used to modify or intercept messages. To intercept messages to add headers or modify message content, create a new or use an existing middleware component.
:::

## Receive Observer

To observe messages as they are received by the transport, create a class that implements the `IReceiveObserver` interface, and connect it to the bus as shown below.

```csharp
public class ReceiveObserver : 
    IReceiveObserver
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

To configure a receive observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation. When a container is not being used, the `ConnectReceiveObserver` bus method can be used instead.

```cs
services.AddReceiveObserver<ReceiveObserver>();
```

```cs
services.AddReceiveObserver(provider => new ReceiveObserver());
```

## Consume Observer

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

To configure a consume observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation. When a container is not being used, the `ConnectConsumeObserver` bus method can be used instead.

```cs
services.AddConsumeObserver<ConsumeObserver>();
```

```cs
services.AddConsumeObserver(provider => new ConsumeObserver());
```

### Message Type Consume Observer

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

## Send Observer

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

## Publish Observer

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

## Bus Observer

To observe bus life cycle events, create a class which implements `IBusObserver`, as shown below.

```csharp
public class BusObserver : 
    IBusObserver
{
    public void PostCreate(IBus bus)
    {
        // called after the bus has been created, but before it has been started.
    }

    public void CreateFaulted(Exception exception)
    {
        // called if the bus creation fails for some reason
    }

    public Task PreStart(IBus bus)
    {
        // called just before the bus is started
    }

    public Task PostStart(IBus bus, Task<BusReady> busReady)
    {
        // called once the bus has been started successfully. The task can be used to wait for
        // all of the receive endpoints to be ready.
    }

    public Task StartFaulted(IBus bus, Exception exception)
    {
        // called if the bus fails to start for some reason (dead battery, no fuel, etc.)
    }

    public Task PreStop(IBus bus)
    {
        // called just before the bus is stopped
    }

    public Task PostStop(IBus bus)
    {
        // called after the bus has been stopped
    }

    public Task StopFaulted(IBus bus, Exception exception)
    {
        // called if the bus fails to stop (no brakes)
    }
}
```

To configure a bus observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation.

```cs
services.AddBusObserver<BusObserver>();
```

```cs
services.AddBusObserver(provider => new BusObserver());
```

## Receive Endpoint Observer

<<< @/src/MassTransit.Abstractions/Observers/IReceiveEndpointObserver.cs

To configure a receive endpoint observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation.

```cs
services.AddReceiveEndpointObserver<ReceiveEndpointObserver>();
```

```cs
services.AddReceiveEndpointObserver(provider => new ReceiveEndpointObserver());
```

