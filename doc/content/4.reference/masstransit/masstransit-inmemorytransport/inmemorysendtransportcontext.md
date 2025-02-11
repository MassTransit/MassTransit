---

title: InMemorySendTransportContext

---

# InMemorySendTransportContext

Namespace: MassTransit.InMemoryTransport

```csharp
public class InMemorySendTransportContext : BaseSendTransportContext, PipeContext, SendTransportContext, ISendObserverConnector, SendTransportContext<PipeContext>, IPipeContextSource<PipeContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [BaseSendTransportContext](../masstransit-transports/basesendtransportcontext) → [InMemorySendTransportContext](../masstransit-inmemorytransport/inmemorysendtransportcontext)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [SendTransportContext](../masstransit-transports/sendtransportcontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [SendTransportContext\<PipeContext\>](../masstransit-transports/sendtransportcontext-1), [IPipeContextSource\<PipeContext\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **EntityName**

```csharp
public string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ActivitySystem**

```csharp
public string ActivitySystem { get; }
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

## Constructors

### **InMemorySendTransportContext(IHostConfiguration, ReceiveEndpointContext, IMessageExchange\<InMemoryTransportMessage\>)**

```csharp
public InMemorySendTransportContext(IHostConfiguration hostConfiguration, ReceiveEndpointContext context, IMessageExchange<InMemoryTransportMessage> exchange)
```

#### Parameters

`hostConfiguration` [IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`exchange` [IMessageExchange\<InMemoryTransportMessage\>](../masstransit-transports-fabric/imessageexchange-1)<br/>

## Methods

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

### **CreateSendContext\<T\>(PipeContext, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<SendContext<T>> CreateSendContext<T>(PipeContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send\<T\>(PipeContext, SendContext\<T\>)**

```csharp
public Task Send<T>(PipeContext transportContext, SendContext<T> sendContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`transportContext` [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)<br/>

`sendContext` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Send(IPipe\<PipeContext\>, CancellationToken)**

```csharp
public Task Send(IPipe<PipeContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`pipe` [IPipe\<PipeContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>
