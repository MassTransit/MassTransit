---

title: OutboxSendEndpointProvider

---

# OutboxSendEndpointProvider

Namespace: MassTransit.Middleware.Outbox

```csharp
public class OutboxSendEndpointProvider : ISendEndpointProvider, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxSendEndpointProvider](../masstransit-middleware-outbox/outboxsendendpointprovider)<br/>
Implements [ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Constructors

### **OutboxSendEndpointProvider(OutboxSendContext, ISendEndpointProvider)**

```csharp
public OutboxSendEndpointProvider(OutboxSendContext outboxContext, ISendEndpointProvider sendEndpointProvider)
```

#### Parameters

`outboxContext` [OutboxSendContext](../masstransit-middleware/outboxsendcontext)<br/>

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
