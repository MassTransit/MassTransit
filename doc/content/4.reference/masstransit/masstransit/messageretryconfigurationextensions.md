---

title: MessageRetryConfigurationExtensions

---

# MessageRetryConfigurationExtensions

Namespace: MassTransit

```csharp
public static class MessageRetryConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageRetryConfigurationExtensions](../masstransit/messageretryconfigurationextensions)

## Methods

### **UseMessageRetry(IConsumePipeConfigurator, Action\<IRetryConfigurator\>)**

For all configured messages type (handlers, consumers, and sagas), configures message retry using the retry configuration specified.
 Retry is configured once for each message type, and is added prior to the consumer factory or saga repository in the pipeline.

```csharp
public static void UseMessageRetry(IConsumePipeConfigurator configurator, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry(IConsumePipeConfigurator, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

For all configured messages type (handlers, consumers, and sagas), configures message retry using the retry configuration specified.
 Retry is configured once for each message type, and is added prior to the consumer factory or saga repository in the pipeline.

```csharp
public static void UseMessageRetry(IConsumePipeConfigurator configurator, IBusFactoryConfigurator connector, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`connector` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, Action\<IRetryConfigurator\>)**

Configures the message retry for the consumer, regardless of message type.

```csharp
public static void UseMessageRetry<TConsumer>(IConsumerConfigurator<TConsumer> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry\<TConsumer\>(IConsumerConfigurator\<TConsumer\>, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

Configures the message retry for the consumer, regardless of message type.

```csharp
public static void UseMessageRetry<TConsumer>(IConsumerConfigurator<TConsumer> configurator, IBusFactoryConfigurator busFactoryConfigurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`busFactoryConfigurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry\<TSaga\>(ISagaConfigurator\<TSaga\>, Action\<IRetryConfigurator\>)**

Configures the message retry for the consumer, regardless of message type.

```csharp
public static void UseMessageRetry<TSaga>(ISagaConfigurator<TSaga> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry\<TSaga\>(ISagaConfigurator\<TSaga\>, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

Configures the message retry for the consumer, regardless of message type.

```csharp
public static void UseMessageRetry<TSaga>(ISagaConfigurator<TSaga> configurator, IBusFactoryConfigurator busFactoryConfigurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`configurator` [ISagaConfigurator\<TSaga\>](../../masstransit-abstractions/masstransit/isagaconfigurator-1)<br/>

`busFactoryConfigurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry\<TMessage\>(IHandlerConfigurator\<TMessage\>, Action\<IRetryConfigurator\>)**

Configures the message retry for the consumer, regardless of message type.

```csharp
public static void UseMessageRetry<TMessage>(IHandlerConfigurator<TMessage> configurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **UseMessageRetry\<TMessage\>(IHandlerConfigurator\<TMessage\>, IBusFactoryConfigurator, Action\<IRetryConfigurator\>)**

Configures the message retry for the consumer, regardless of message type.

```csharp
public static void UseMessageRetry<TMessage>(IHandlerConfigurator<TMessage> configurator, IBusFactoryConfigurator busFactoryConfigurator, Action<IRetryConfigurator> configure)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1)<br/>

`busFactoryConfigurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
The bus factory configurator, to connect the observer, to cancel retries if the bus is stopped

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
