---

title: BaseSendTransportContext

---

# BaseSendTransportContext

Namespace: MassTransit.Transports

```csharp
public abstract class BaseSendTransportContext : BasePipeContext, PipeContext, SendTransportContext, ISendObserverConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [BaseSendTransportContext](../masstransit-transports/basesendtransportcontext)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [SendTransportContext](../masstransit-transports/sendtransportcontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector)

## Properties

### **EntityName**

```csharp
public abstract string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LogContext**

```csharp
public ILogContext LogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **ActivityName**

```csharp
public string ActivityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivityDestination**

```csharp
public string ActivityDestination { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivitySystem**

```csharp
public abstract string ActivitySystem { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SendObservers**

```csharp
public SendObservable SendObservers { get; }
```

#### Property Value

[SendObservable](../../masstransit-abstractions/masstransit-observables/sendobservable)<br/>

### **Serialization**

```csharp
public ISerialization Serialization { get; }
```

#### Property Value

[ISerialization](../../masstransit-abstractions/masstransit/iserialization)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **CreateSendContext\<T\>(T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public abstract Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConnectSendObserver(ISendObserver)**

```csharp
public ConnectHandle ConnectSendObserver(ISendObserver observer)
```

#### Parameters

`observer` [ISendObserver](../../masstransit-abstractions/masstransit/isendobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **GetAgentHandles()**

```csharp
public IEnumerable<IAgent> GetAgentHandles()
```

#### Returns

[IEnumerable\<IAgent\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
