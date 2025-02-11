---

title: InMemoryTransportProvider

---

# InMemoryTransportProvider

Namespace: MassTransit.InMemoryTransport

```csharp
public sealed class InMemoryTransportProvider : Agent, IAgent, IInMemoryTransportProvider, InMemoryTransportContext, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [InMemoryTransportProvider](../masstransit-inmemorytransport/inmemorytransportprovider)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [IInMemoryTransportProvider](../masstransit-inmemorytransport/iinmemorytransportprovider), [InMemoryTransportContext](../masstransit-inmemorytransport/inmemorytransportcontext), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **MessageFabric**

```csharp
public IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric { get; }
```

#### Property Value

[IMessageFabric\<InMemoryTransportContext, InMemoryTransportMessage\>](../masstransit-transports-fabric/imessagefabric-2)<br/>

### **Ready**

```csharp
public Task Ready { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Completed**

```csharp
public Task Completed { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Stopping**

```csharp
public CancellationToken Stopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **Stopped**

```csharp
public CancellationToken Stopped { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Constructors

### **InMemoryTransportProvider(IInMemoryHostConfiguration, IInMemoryTopologyConfiguration)**

```csharp
public InMemoryTransportProvider(IInMemoryHostConfiguration hostConfiguration, IInMemoryTopologyConfiguration topologyConfiguration)
```

#### Parameters

`hostConfiguration` [IInMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryhostconfiguration)<br/>

`topologyConfiguration` [IInMemoryTopologyConfiguration](../masstransit-inmemorytransport-configuration/iinmemorytopologyconfiguration)<br/>

## Methods

### **CreateSendTransport(ReceiveEndpointContext, Uri)**

```csharp
public Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext receiveEndpointContext, Uri address)
```

#### Parameters

`receiveEndpointContext` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`address` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **NormalizeAddress(Uri)**

```csharp
public Uri NormalizeAddress(Uri address)
```

#### Parameters

`address` Uri<br/>

#### Returns

Uri<br/>

### **CreatePublishTransport\<T\>(ReceiveEndpointContext, Uri)**

```csharp
public Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext receiveEndpointContext, Uri publishAddress)
```

#### Type Parameters

`T`<br/>

#### Parameters

`receiveEndpointContext` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`publishAddress` Uri<br/>

#### Returns

[Task\<ISendTransport\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
