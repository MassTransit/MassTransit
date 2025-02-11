---

title: InMemoryOutboxPublishEndpointProvider

---

# InMemoryOutboxPublishEndpointProvider

Namespace: MassTransit.Middleware.InMemoryOutbox

```csharp
public class InMemoryOutboxPublishEndpointProvider : IPublishEndpointProvider, IPublishObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxPublishEndpointProvider](../masstransit-middleware-inmemoryoutbox/inmemoryoutboxpublishendpointprovider)<br/>
Implements [IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector)

## Constructors

### **InMemoryOutboxPublishEndpointProvider(OutboxContext, IPublishEndpointProvider)**

```csharp
public InMemoryOutboxPublishEndpointProvider(OutboxContext outboxContext, IPublishEndpointProvider publishEndpointProvider)
```

#### Parameters

`outboxContext` [OutboxContext](../masstransit-middleware-inmemoryoutbox/outboxcontext)<br/>

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
