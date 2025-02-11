---

title: OutboxSendEndpoint

---

# OutboxSendEndpoint

Namespace: MassTransit.Middleware.InMemoryOutbox

```csharp
public class OutboxSendEndpoint : ITransportSendEndpoint, ISendEndpoint, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxSendEndpoint](../masstransit-middleware-inmemoryoutbox/outboxsendendpoint)<br/>
Implements [ITransportSendEndpoint](../masstransit-transports/itransportsendendpoint), [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **Endpoint**

The actual endpoint, wrapped by the outbox

```csharp
public ISendEndpoint Endpoint { get; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

## Constructors

### **OutboxSendEndpoint(OutboxContext, ISendEndpoint)**

Creates an send endpoint on the outbox

```csharp
public OutboxSendEndpoint(OutboxContext outboxContext, ISendEndpoint endpoint)
```

#### Parameters

`outboxContext` [OutboxContext](../masstransit-middleware-inmemoryoutbox/outboxcontext)<br/>
The outbox context for this consume operation

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>
The actual endpoint returned by the transport

## Methods

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **CreateSendContext\<T\>(T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
