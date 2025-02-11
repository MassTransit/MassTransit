---

title: ScheduledRedeliveryConfigurationExtensions

---

# ScheduledRedeliveryConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ScheduledRedeliveryConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScheduledRedeliveryConfigurationExtensions](../masstransit/scheduledredeliveryconfigurationextensions)

## Methods

### **UseScheduledRedelivery\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Action\<IRetryConfigurator\>)**

Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy, via
 the delayed exchange feature of ActiveMQ.

```csharp
public static void UseScheduledRedelivery<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseScheduledRedelivery\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, IRetryPolicy)**

Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy.

```csharp
public static void UseScheduledRedelivery<T>(IPipeConfigurator<ConsumeContext<T>> configurator, IRetryPolicy retryPolicy)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **UseScheduledRedelivery(IConsumePipeConfigurator, Action\<IRetryConfigurator\>)**

Configure scheduled redelivery for all message types

```csharp
public static void UseScheduledRedelivery(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configureRetry)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`configureRetry` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseScheduledRedelivery\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, Action\<IRetryConfigurator\>)**

Configure scheduled redelivery for the consumer, regardless of message type.

```csharp
public static void UseScheduledRedelivery<TConsumer>(IConsumerConfigurator<TConsumer> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseScheduledRedelivery\<TSaga\>(ISagaConfigurator\<TSaga\>, Action\<IRetryConfigurator\>)**

Configure scheduled redelivery for the saga, regardless of message type.

```csharp
public static void UseScheduledRedelivery<TSaga>(ISagaConfigurator<TSaga> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseScheduledRedelivery\<TMessage\>(IHandlerConfigurator\<TMessage\>, Action\<IRetryConfigurator\>)**

Configures the message retry for the handler, regardless of message type.

```csharp
public static void UseScheduledRedelivery<TMessage>(IHandlerConfigurator<TMessage> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
