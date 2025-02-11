---

title: IConsumerMessageSpecification<TConsumer, TMessage>

---

# IConsumerMessageSpecification\<TConsumer, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface IConsumerMessageSpecification<TConsumer, TMessage> : IConsumerMessageSpecification<TConsumer>, IPipeConfigurator<ConsumerConsumeContext<TConsumer>>, IConsumerConfigurationObserverConnector, ISpecification, IConsumerMessageConfigurator<TConsumer, TMessage>, IPipeConfigurator<ConsumerConsumeContext<TConsumer, TMessage>>, IConsumerMessageConfigurator<TMessage>, IPipeConfigurator<ConsumeContext<TMessage>>
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

Implements [IConsumerMessageSpecification\<TConsumer\>](../masstransit-configuration/iconsumermessagespecification-1), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/iconsumerconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IConsumerMessageConfigurator\<TConsumer, TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-2), [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IConsumerMessageConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/iconsumermessageconfigurator-1), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Methods

### **Build(IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>)**

```csharp
IPipe<ConsumerConsumeContext<TConsumer, TMessage>> Build(IFilter<ConsumerConsumeContext<TConsumer, TMessage>> consumeFilter)
```

#### Parameters

`consumeFilter` [IFilter\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[IPipe\<ConsumerConsumeContext\<TConsumer, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

### **BuildMessagePipe(Action\<IPipeConfigurator\<ConsumeContext\<TMessage\>\>\>)**

Configure the message pipe as it is built. Any previously configured filters will precede
 the configuration applied by the  callback.

```csharp
IPipe<ConsumeContext<TMessage>> BuildMessagePipe(Action<IPipeConfigurator<ConsumeContext<TMessage>>> configure)
```

#### Parameters

`configure` [Action\<IPipeConfigurator\<ConsumeContext\<TMessage\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the message pipe

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
