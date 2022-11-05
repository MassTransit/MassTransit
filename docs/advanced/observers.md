---
sidebarDepth: 1
---

# Observers

MassTransit supports several message observers allowing received, consumed, sent, and published messages to be monitored. There is a bus observer as well, so that the bus life cycle can be monitored.

::: warning
Observers should not be used to modify or intercept messages. To intercept messages to add headers or modify message content, create a new or use an existing middleware component.
:::

## Receive

To observe messages as they are received by the transport, create a class that implements the `IReceiveObserver` interface, and connect it to the bus as shown below.

<<< @/src/MassTransit.Abstractions/Observers/IReceiveObserver.cs

To configure a receive observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation. When a container is not being used, the `ConnectReceiveObserver` bus method can be used instead.

```cs
services.AddReceiveObserver<ReceiveObserver>();
```

```cs
services.AddReceiveObserver(provider => new ReceiveObserver());
```

## Consume

If the `ReceiveContext` isn't fascinating enough for you, perhaps the actual consumption of messages might float your boat. A consume observer implements the `IConsumeObserver` interface, as shown below.

<<< @/src/MassTransit.Abstractions/Observers/IConsumeObserver.cs

To configure a consume observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation. When a container is not being used, the `ConnectConsumeObserver` bus method can be used instead.

```cs
services.AddConsumeObserver<ConsumeObserver>();
```

```cs
services.AddConsumeObserver(provider => new ConsumeObserver());
```

### Consume Message

Okay, so it's obvious that if you've read this far you want a more specific observer, one that only is called when a specific message type is consumed. We have you covered there too, as shown below.

<<< @/src/MassTransit.Abstractions/Observers/IConsumeMessageObserver.cs

To connect the observer, use the `ConnectConsumeMessageObserver` method before starting the bus.

> The `ConsumeObserver<T>` interface may be deprecated at some point, it's sort of a legacy observer that isn't recommended.

## Send

Okay, so, incoming messages are not your thing. We get it, you're all about what goes out. It's cool. It's better to send than to receive. Or is that give? Anyway, a send observer is also available.

<<< @/src/MassTransit.Abstractions/Observers/ISendObserver.cs

To configure a send observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer
creation. When a container is not being used, the `ConnectSendObserver` bus method can be used instead.

```cs
services.AddSendObserver<SendObserver>();
```

```cs
services.AddSendObserver(provider => new SendObserver());
```

## Publish

In addition to send, publish is also observable. Because the semantics matter, absolutely. Using the MessageId to link them up as it's unique for each message. Remember that Publish and Send are two distinct operations so if you want to observe all messages that are leaving your service, you have to connect both Publish and Send observers.

<<< @/src/MassTransit.Abstractions/Observers/IPublishObserver.cs

To configure a public observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer
creation. When a container is not being used, the `ConnectPublishObserver` bus method can be used instead.

```cs
services.AddPublishObserver<PublishObserver>();
```

```cs
services.AddPublishObserver(provider => new PublishObserver());
```

## Bus

To observe bus life cycle events, create a class which implements `IBusObserver`, as shown below.

<<< @/src/MassTransit.Abstractions/Observers/IBusObserver.cs

To configure a bus observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation.

```cs
services.AddBusObserver<BusObserver>();
```

```cs
services.AddBusObserver(provider => new BusObserver());
```

## Receive Endpoint

<<< @/src/MassTransit.Abstractions/Observers/IReceiveEndpointObserver.cs

To configure a receive endpoint observer, add it to the container using one of the methods shown below. The factory method version allows customization of the observer creation.

```cs
services.AddReceiveEndpointObserver<ReceiveEndpointObserver>();
```

```cs
services.AddReceiveEndpointObserver(provider => new ReceiveEndpointObserver());
```

## State Machine Event

To observe events consumed by a saga state machine, use an `IEventObserver<T>` where `T` is the saga instance type.

<<< @/src/MassTransit.Abstractions/SagaStateMachine/IEventObserver.cs

To configure an event observer, add it to the container using one of the methods shown below. The factory method version allows customization of the
observer creation.

```cs
services.AddEventObserver<T, EventObserver<T>>();
```

```cs
services.AddEventObserver<T>(provider => new EventObserver<T>());
```

## State Machine State

To observe state changes that happen in a saga state machine, use an `IStateObserver<T>` where `T` is the saga instance type.

<<< @/src/MassTransit.Abstractions/SagaStateMachine/IStateObserver.cs

To configure a state observer, add it to the container using one of the methods shown below. The factory method version allows customization of the
observer creation.

```cs
services.AddStateObserver<T, StateObserver<T>>();
```

```cs
services.AddStateObserver<T>(provider => new StateObserver<T>());
```

