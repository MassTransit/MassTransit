---

title: MessageRetryConsumerConfigurationObserver<TConsumer>

---

# MessageRetryConsumerConfigurationObserver\<TConsumer\>

Namespace: MassTransit.Configuration

Configures a message retry for a consumer, on the consumer configurator, which is constrained to
 the message types for that consumer, and only applies to the consumer prior to the consumer factory.

```csharp
public class MessageRetryConsumerConfigurationObserver<TConsumer> : IConsumerConfigurationObserver
```

#### Type Parameters

`TConsumer`<br/>
The consumer type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageRetryConsumerConfigurationObserver\<TConsumer\>](../masstransit-configuration/messageretryconsumerconfigurationobserver-1)<br/>
Implements [IConsumerConfigurationObserver](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserver)

## Constructors

### **MessageRetryConsumerConfigurationObserver(IConsumerConfigurator\<TConsumer\>, CancellationToken, Action\<IRetryConfigurator\>)**

```csharp
public MessageRetryConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, CancellationToken cancellationToken, Action<IRetryConfigurator> configure)
```

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Methods

### **BatchConsumerConfigured\<TMessage\>(IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>)**

```csharp
public void BatchConsumerConfigured<TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<TConsumer, Batch\<TMessage\>\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2)<br/>
