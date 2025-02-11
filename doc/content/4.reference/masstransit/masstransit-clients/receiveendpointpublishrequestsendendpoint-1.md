---

title: ReceiveEndpointPublishRequestSendEndpoint<TRequest>

---

# ReceiveEndpointPublishRequestSendEndpoint\<TRequest\>

Namespace: MassTransit.Clients

```csharp
public class ReceiveEndpointPublishRequestSendEndpoint<TRequest> : RequestSendEndpoint<TRequest>, IRequestSendEndpoint<TRequest>
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestSendEndpoint\<TRequest\>](../masstransit-clients/requestsendendpoint-1) → [ReceiveEndpointPublishRequestSendEndpoint\<TRequest\>](../masstransit-clients/receiveendpointpublishrequestsendendpoint-1)<br/>
Implements [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)

## Constructors

### **ReceiveEndpointPublishRequestSendEndpoint(HostReceiveEndpointHandle, ConsumeContext)**

```csharp
public ReceiveEndpointPublishRequestSendEndpoint(HostReceiveEndpointHandle handle, ConsumeContext consumeContext)
```

#### Parameters

`handle` [HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Methods

### **GetSendEndpoint()**

```csharp
protected Task<ISendEndpoint> GetSendEndpoint()
```

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
