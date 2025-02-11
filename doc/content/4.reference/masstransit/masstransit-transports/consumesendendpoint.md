---

title: ConsumeSendEndpoint

---

# ConsumeSendEndpoint

Namespace: MassTransit.Transports

Intercepts the ISendEndpoint and makes it part of the current consume context

```csharp
public class ConsumeSendEndpoint : SendEndpointProxy, ITransportSendEndpoint, ISendEndpoint, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SendEndpointProxy](../masstransit-transports/sendendpointproxy) → [ConsumeSendEndpoint](../masstransit-transports/consumesendendpoint)<br/>
Implements [ITransportSendEndpoint](../masstransit-transports/itransportsendendpoint), [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **Endpoint**

```csharp
public ISendEndpoint Endpoint { get; }
```

#### Property Value

[ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

## Constructors

### **ConsumeSendEndpoint(ISendEndpoint, ConsumeContext, Nullable\<Guid\>)**

```csharp
public ConsumeSendEndpoint(ISendEndpoint endpoint, ConsumeContext context, Nullable<Guid> requestId)
```

#### Parameters

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`requestId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

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

### **GetPipeProxy\<T\>(IPipe\<SendContext\<T\>\>)**

```csharp
protected IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
