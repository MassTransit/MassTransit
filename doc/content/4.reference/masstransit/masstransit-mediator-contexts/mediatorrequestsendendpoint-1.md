---

title: MediatorRequestSendEndpoint<TRequest>

---

# MediatorRequestSendEndpoint\<TRequest\>

Namespace: MassTransit.Mediator.Contexts

```csharp
public class MediatorRequestSendEndpoint<TRequest> : RequestSendEndpoint<TRequest>, IRequestSendEndpoint<TRequest>
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestSendEndpoint\<TRequest\>](../masstransit-clients/requestsendendpoint-1) → [MediatorRequestSendEndpoint\<TRequest\>](../masstransit-mediator-contexts/mediatorrequestsendendpoint-1)<br/>
Implements [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)

## Constructors

### **MediatorRequestSendEndpoint(ISendEndpoint, ConsumeContext)**

```csharp
public MediatorRequestSendEndpoint(ISendEndpoint endpoint, ConsumeContext consumeContext)
```

#### Parameters

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Methods

### **GetSendEndpoint()**

```csharp
protected Task<ISendEndpoint> GetSendEndpoint()
```

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
