---

title: ConsumerConsumeContext<TConsumer>

---

# ConsumerConsumeContext\<TConsumer\>

Namespace: MassTransit

```csharp
public interface ConsumerConsumeContext<TConsumer> : ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TConsumer`<br/>

Implements [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Consumer**

The consumer which will handle the message

```csharp
public abstract TConsumer Consumer { get; }
```

#### Property Value

TConsumer<br/>
