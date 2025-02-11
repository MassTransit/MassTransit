---

title: ReceiveEndpointSendRequestSendEndpoint<TRequest>

---

# ReceiveEndpointSendRequestSendEndpoint\<TRequest\>

Namespace: MassTransit.Clients

```csharp
public class ReceiveEndpointSendRequestSendEndpoint<TRequest> : RequestSendEndpoint<TRequest>, IRequestSendEndpoint<TRequest>
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestSendEndpoint\<TRequest\>](../masstransit-clients/requestsendendpoint-1) → [ReceiveEndpointSendRequestSendEndpoint\<TRequest\>](../masstransit-clients/receiveendpointsendrequestsendendpoint-1)<br/>
Implements [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)

## Constructors

### **ReceiveEndpointSendRequestSendEndpoint(HostReceiveEndpointHandle, Uri, ConsumeContext)**

```csharp
public ReceiveEndpointSendRequestSendEndpoint(HostReceiveEndpointHandle handle, Uri destinationAddress, ConsumeContext consumeContext)
```

#### Parameters

`handle` [HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

`destinationAddress` Uri<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Methods

### **GetSendEndpoint()**

```csharp
protected Task<ISendEndpoint> GetSendEndpoint()
```

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
