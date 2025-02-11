---

title: SendRequestSendEndpoint<TRequest>

---

# SendRequestSendEndpoint\<TRequest\>

Namespace: MassTransit.Clients

```csharp
public class SendRequestSendEndpoint<TRequest> : RequestSendEndpoint<TRequest>, IRequestSendEndpoint<TRequest>
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestSendEndpoint\<TRequest\>](../masstransit-clients/requestsendendpoint-1) → [SendRequestSendEndpoint\<TRequest\>](../masstransit-clients/sendrequestsendendpoint-1)<br/>
Implements [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)

## Constructors

### **SendRequestSendEndpoint(ISendEndpointProvider, Uri, ConsumeContext)**

```csharp
public SendRequestSendEndpoint(ISendEndpointProvider provider, Uri destinationAddress, ConsumeContext consumeContext)
```

#### Parameters

`provider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

`destinationAddress` Uri<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Methods

### **GetSendEndpoint()**

```csharp
protected Task<ISendEndpoint> GetSendEndpoint()
```

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
