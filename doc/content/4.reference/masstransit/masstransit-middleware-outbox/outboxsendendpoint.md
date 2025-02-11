---

title: OutboxSendEndpoint

---

# OutboxSendEndpoint

Namespace: MassTransit.Middleware.Outbox

```csharp
public class OutboxSendEndpoint : ITransportSendEndpoint, ISendEndpoint, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutboxSendEndpoint](../masstransit-middleware-outbox/outboxsendendpoint)<br/>
Implements [ITransportSendEndpoint](../masstransit-transports/itransportsendendpoint), [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **Endpoint**

```csharp
public ISendEndpoint Endpoint { get; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

## Constructors

### **OutboxSendEndpoint(OutboxSendContext, ISendEndpoint)**

Creates an send endpoint on the outbox

```csharp
public OutboxSendEndpoint(OutboxSendContext outboxContext, ISendEndpoint endpoint)
```

#### Parameters

`outboxContext` [OutboxSendContext](../masstransit-middleware/outboxsendcontext)<br/>
The outbox context for this consume operation

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

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

### **Send\<T\>(T, CancellationToken)**

```csharp
public Task Send<T>(T message, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, CancellationToken)**

```csharp
public Task Send(object message, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, Type, CancellationToken)**

```csharp
public Task Send(object message, Type messageType, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(T, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(Object, Type, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`message` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(Object, CancellationToken)**

```csharp
public Task Send<T>(object values, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(Object, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send\<T\>(Object, IPipe\<SendContext\>, CancellationToken)**

```csharp
public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

`pipe` [IPipe\<SendContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
