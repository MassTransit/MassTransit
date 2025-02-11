---

title: ISagaMessageSpecification<TSaga, TMessage>

---

# ISagaMessageSpecification\<TSaga, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public interface ISagaMessageSpecification<TSaga, TMessage> : ISagaMessageSpecification<TSaga>, IPipeConfigurator<SagaConsumeContext<TSaga>>, ISagaConfigurationObserverConnector, ISpecification, ISagaMessageConfigurator<TSaga, TMessage>, IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>>, ISagaMessageConfigurator<TMessage>, IPipeConfigurator<ConsumeContext<TMessage>>
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

Implements [ISagaMessageSpecification\<TSaga\>](../masstransit-configuration/isagamessagespecification-1), [IPipeConfigurator\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISagaConfigurationObserverConnector](../../masstransit-abstractions/masstransit/isagaconfigurationobserverconnector), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [ISagaMessageConfigurator\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/isagamessageconfigurator-2), [IPipeConfigurator\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [ISagaMessageConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/isagamessageconfigurator-1), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Methods

### **BuildConsumerPipe(IFilter\<SagaConsumeContext\<TSaga, TMessage\>\>)**

Build the consumer pipe, using the consume filter specified.

```csharp
IPipe<SagaConsumeContext<TSaga, TMessage>> BuildConsumerPipe(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter)
```

#### Parameters

`consumeFilter` [IFilter\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>

#### Returns

[IPipe\<SagaConsumeContext\<TSaga, TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

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
