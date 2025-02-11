---

title: PublishRequestSendEndpoint<TRequest>

---

# PublishRequestSendEndpoint\<TRequest\>

Namespace: MassTransit.Clients

```csharp
public class PublishRequestSendEndpoint<TRequest> : RequestSendEndpoint<TRequest>, IRequestSendEndpoint<TRequest>
```

#### Type Parameters

`TRequest`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RequestSendEndpoint\<TRequest\>](../masstransit-clients/requestsendendpoint-1) → [PublishRequestSendEndpoint\<TRequest\>](../masstransit-clients/publishrequestsendendpoint-1)<br/>
Implements [IRequestSendEndpoint\<TRequest\>](../../masstransit-abstractions/masstransit/irequestsendendpoint-1)

## Constructors

### **PublishRequestSendEndpoint(IPublishEndpointProvider, ConsumeContext)**

```csharp
public PublishRequestSendEndpoint(IPublishEndpointProvider provider, ConsumeContext consumeContext)
```

#### Parameters

`provider` [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

`consumeContext` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

## Methods

### **GetSendEndpoint()**

```csharp
protected Task<ISendEndpoint> GetSendEndpoint()
```

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
