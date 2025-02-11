---

title: DelayedRedeliveryExtensions

---

# DelayedRedeliveryExtensions

Namespace: MassTransit

```csharp
public static class DelayedRedeliveryExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DelayedRedeliveryExtensions](../masstransit/delayedredeliveryextensions)

## Methods

### **UseDelayedRedelivery\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Action\<IRedeliveryConfigurator\>)**

Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy, via
 the delayed exchange feature of ActiveMQ.

```csharp
public static void UseDelayedRedelivery<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRedeliveryConfigurator> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseDelayedRedelivery\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, IRetryPolicy)**

Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy.

```csharp
public static void UseDelayedRedelivery<T>(IPipeConfigurator<ConsumeContext<T>> configurator, IRetryPolicy retryPolicy)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`retryPolicy` [IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **UseDelayedRedelivery(IConsumePipeConfigurator, Action\<IRedeliveryConfigurator\>)**

For all configured messages type (handlers, consumers, and sagas), configures delayed redelivery using the retry configuration specified.
 Redelivery is configured once for each message type, and is added prior to the consumer factory or saga repository in the pipeline.

```csharp
public static void UseDelayedRedelivery(IConsumePipeConfigurator configurator, Action<IRedeliveryConfigurator> configureRetry)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`configureRetry` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseDelayedRedelivery\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, Action\<IRedeliveryConfigurator\>)**

Configure scheduled redelivery for the consumer, regardless of message type.

```csharp
public static void UseDelayedRedelivery<TConsumer>(IConsumerConfigurator<TConsumer> configurator, Action<IRedeliveryConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseDelayedRedelivery\<TSaga\>(ISagaConfigurator\<TSaga\>, Action\<IRedeliveryConfigurator\>)**

Configure scheduled redelivery for the saga, regardless of message type.

```csharp
public static void UseDelayedRedelivery<TSaga>(ISagaConfigurator<TSaga> configurator, Action<IRedeliveryConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseDelayedRedelivery\<TMessage\>(IHandlerConfigurator\<TMessage\>, Action\<IRedeliveryConfigurator\>)**

Configures the message retry for the handler, regardless of message type.

```csharp
public static void UseDelayedRedelivery<TMessage>(IHandlerConfigurator<TMessage> configurator, Action<IRedeliveryConfigurator> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`configure` [Action\<IRedeliveryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
