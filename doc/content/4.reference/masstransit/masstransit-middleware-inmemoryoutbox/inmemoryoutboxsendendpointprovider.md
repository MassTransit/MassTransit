---

title: InMemoryOutboxSendEndpointProvider

---

# InMemoryOutboxSendEndpointProvider

Namespace: MassTransit.Middleware.InMemoryOutbox

```csharp
public class InMemoryOutboxSendEndpointProvider : ISendEndpointProvider, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryOutboxSendEndpointProvider](../masstransit-middleware-inmemoryoutbox/inmemoryoutboxsendendpointprovider)<br/>
Implements [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Constructors

### **InMemoryOutboxSendEndpointProvider(OutboxContext, ISendEndpointProvider)**

```csharp
public InMemoryOutboxSendEndpointProvider(OutboxContext outboxContext, ISendEndpointProvider sendEndpointProvider)
```

#### Parameters

`outboxContext` [OutboxContext](../masstransit-middleware-inmemoryoutbox/outboxcontext)<br/>

`sendEndpointProvider` [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

## Methods

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetSendEndpoint(Uri)**

```csharp
public Task<ISendEndpoint> GetSendEndpoint(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

[Task\<ISendEndpoint\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
