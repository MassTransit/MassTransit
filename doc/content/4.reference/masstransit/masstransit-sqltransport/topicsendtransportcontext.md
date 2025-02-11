---

title: TopicSendTransportContext

---

# TopicSendTransportContext

Namespace: MassTransit.SqlTransport

```csharp
public class TopicSendTransportContext : BaseSendTransportContext, PipeContext, SendTransportContext, ISendObserverConnector, SendTransportContext<ClientContext>, IPipeContextSource<ClientContext>, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [BaseSendTransportContext](../masstransit-transports/basesendtransportcontext) → [TopicSendTransportContext](../masstransit-sqltransport/topicsendtransportcontext)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [SendTransportContext](../masstransit-transports/sendtransportcontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [SendTransportContext\<ClientContext\>](../masstransit-transports/sendtransportcontext-1), [IPipeContextSource\<ClientContext\>](../../masstransit-abstractions/masstransit/ipipecontextsource-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

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

### **TopicSendTransportContext(ISqlHostConfiguration, ReceiveEndpointContext, IClientContextSupervisor, IPipe\<ClientContext\>, String)**

```csharp
public TopicSendTransportContext(ISqlHostConfiguration hostConfiguration, ReceiveEndpointContext receiveEndpointContext, IClientContextSupervisor supervisor, IPipe<ClientContext> configureTopologyPipe, string entityName)
```

#### Parameters

`hostConfiguration` [ISqlHostConfiguration](../masstransit-sqltransport-configuration/isqlhostconfiguration)<br/>

`receiveEndpointContext` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`supervisor` [IClientContextSupervisor](../masstransit-sqltransport/iclientcontextsupervisor)<br/>

`configureTopologyPipe` [IPipe\<ClientContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`entityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

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

### **GetAgentHandles()**

```csharp
public IEnumerable<IAgent> GetAgentHandles()
```

#### Returns

[IEnumerable\<IAgent\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Send(IPipe\<ClientContext\>, CancellationToken)**

```csharp
public Task Send(IPipe<ClientContext> pipe, CancellationToken cancellationToken)
```

#### Parameters

`pipe` [IPipe\<ClientContext\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **CreateSendContext\<T\>(ClientContext, T, IPipe\<SendContext\<T\>\>, CancellationToken)**

```csharp
public Task<SendContext<T>> CreateSendContext<T>(ClientContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ClientContext](../masstransit-sqltransport/clientcontext)<br/>

`message` T<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<SendContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Send\<T\>(ClientContext, SendContext\<T\>)**

```csharp
public Task Send<T>(ClientContext clientContext, SendContext<T> sendContext)
```

#### Type Parameters

`T`<br/>

#### Parameters

`clientContext` [ClientContext](../masstransit-sqltransport/clientcontext)<br/>

`sendContext` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
