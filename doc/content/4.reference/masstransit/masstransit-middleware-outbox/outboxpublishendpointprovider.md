---

title: OutboxPublishEndpointProvider

---

# OutboxPublishEndpointProvider

Namespace: MassTransit.Middleware.Outbox

```csharp
public class OutboxPublishEndpointProvider : IPublishEndpointProvider, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxPublishEndpointProvider](../masstransit-middleware-outbox/outboxpublishendpointprovider)<br/>
Implements [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Constructors

### **OutboxPublishEndpointProvider(OutboxSendContext, IPublishEndpointProvider)**

```csharp
public OutboxPublishEndpointProvider(OutboxSendContext context, IPublishEndpointProvider publishEndpointProvider)
```

#### Parameters

`context` [OutboxSendContext](../masstransit-middleware/outboxsendcontext)<br/>

`publishEndpointProvider` [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

## Methods

### **ConnectPublishObserver(IPublishObserver)**

```csharp
public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
```

#### Parameters

`observer` [IPublishObserver](../../masstransit-abstractions/masstransit/ipublishobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetPublishSendEndpoint\<T\>()**

```csharp
public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
