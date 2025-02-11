---

title: JobContext<TMessage>

---

# JobContext\<TMessage\>

Namespace: MassTransit

```csharp
public interface JobContext<TMessage> : JobContext, PipeContext, MessageContext, ISendEndpointProvider, ISendObserverConnector, IPublishEndpoint, IPublishObserverConnector
```

#### Type Parameters

`TMessage`<br/>

Implements [JobContext](../masstransit/jobcontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector)

## Properties

### **Job**

The message that initiated the job

```csharp
public abstract TMessage Job { get; }
```

#### Property Value

TMessage<br/>
