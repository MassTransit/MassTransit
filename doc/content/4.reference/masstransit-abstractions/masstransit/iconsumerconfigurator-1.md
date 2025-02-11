---

title: IConsumerConfigurator<TConsumer>

---

# IConsumerConfigurator\<TConsumer\>

Namespace: MassTransit

```csharp
public interface IConsumerConfigurator<TConsumer> : IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurator, IConsumeConfigurator, IConsumerConfigurationObserverConnector, IOptionsSet
```

#### Type Parameters

`TConsumer`<br/>

Implements [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../masstransit/ipipeconfigurator-1), [IConsumerConfigurator](../masstransit/iconsumerconfigurator), [IConsumeConfigurator](../masstransit/iconsumeconfigurator), [IConsumerConfigurationObserverConnector](../masstransit/iconsumerconfigurationobserverconnector), [IOptionsSet](../masstransit-configuration/ioptionsset)

## Methods

### **Message\<T\>(Action\<IConsumerMessageConfigurator\<T\>\>)**

Add middleware to the message pipeline, which is invoked prior to the consumer factory.

```csharp
void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to configure the message pipeline

### **ConsumerMessage\<T\>(Action\<IConsumerMessageConfigurator\<TConsumer, T\>\>)**

Add middleware to the consumer pipeline, for the specified message type, which is invoked
 after the consumer factory.

```csharp
void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configure` [Action\<IConsumerMessageConfigurator\<TConsumer, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The callback to configure the message pipeline
