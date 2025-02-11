---

title: IConsumerSpecification<TConsumer>

---

# IConsumerSpecification\<TConsumer\>

Namespace: MassTransit.Configuration

A consumer specification, that can be modified

```csharp
public interface IConsumerSpecification<TConsumer> : IConsumerConfigurator<TConsumer>, IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurator, IConsumeConfigurator, IConsumerConfigurationObserverConnector, IOptionsSet, ISpecification
```

#### Type Parameters

`TConsumer`<br/>

Implements [IConsumerConfigurator\<TConsumer\>](../../masstransit-abstractions/masstransit/iconsumerconfigurator-1), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurator](../../masstransit-abstractions/masstransit/iconsumerconfigurator), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [IOptionsSet](../../masstransit-abstractions/masstransit-configuration/ioptionsset), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Methods

### **GetMessageSpecification\<T\>()**

```csharp
IConsumerMessageSpecification<TConsumer, T> GetMessageSpecification<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerMessageSpecification\<TConsumer, T\>](../masstransit-configuration/iconsumermessagespecification-2)<br/>

### **ConfigureMessagePipe\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>)**

Apply any consumer-wide configurations to the message pipe, such as concurrency limit, etc.

```csharp
void ConfigureMessagePipe<T>(IPipeConfigurator<ConsumeContext<T>> pipeConfigurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipeConfigurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
