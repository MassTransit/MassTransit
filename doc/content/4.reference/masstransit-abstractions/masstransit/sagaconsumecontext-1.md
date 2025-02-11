---

title: SagaConsumeContext<TSaga>

---

# SagaConsumeContext\<TSaga\>

Namespace: MassTransit

Consume context including the saga instance consuming the message. Note
 this does not expose the message type, for filters that do not care about message type.

```csharp
public interface SagaConsumeContext<TSaga> : ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`TSaga`<br/>
The saga type

Implements [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Saga**

The saga instance for the current consume operation

```csharp
public abstract TSaga Saga { get; }
```

#### Property Value

TSaga<br/>

### **IsCompleted**

True if the saga has been completed, signaling that the repository may remove it.

```csharp
public abstract bool IsCompleted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **SetCompleted()**

Mark the saga instance as completed, which may remove it from the repository or archive it, etc.
 Once completed, a saga instance should never again be visible, even if the same CorrelationId is
 specified.

```csharp
Task SetCompleted()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
