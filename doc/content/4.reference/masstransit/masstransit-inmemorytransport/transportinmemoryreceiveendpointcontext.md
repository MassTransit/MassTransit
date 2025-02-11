---

title: TransportInMemoryReceiveEndpointContext

---

# TransportInMemoryReceiveEndpointContext

Namespace: MassTransit.InMemoryTransport

```csharp
public class TransportInMemoryReceiveEndpointContext : BaseReceiveEndpointContext, PipeContext, ReceiveEndpointContext, ISendObserverConnector, IPublishObserverConnector, IReceiveTransportObserverConnector, IReceiveObserverConnector, IReceiveEndpointObserverConnector, IProbeSite, InMemoryReceiveEndpointContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BasePipeContext](../../masstransit-abstractions/masstransit-middleware/basepipecontext) → [BaseReceiveEndpointContext](../masstransit-transports/basereceiveendpointcontext) → [TransportInMemoryReceiveEndpointContext](../masstransit-inmemorytransport/transportinmemoryreceiveendpointcontext)<br/>
Implements [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext), [ISendObserverConnector](../../masstransit-abstractions/masstransit/isendobserverconnector), [IPublishObserverConnector](../../masstransit-abstractions/masstransit/ipublishobserverconnector), [IReceiveTransportObserverConnector](../../masstransit-abstractions/masstransit/ireceivetransportobserverconnector), [IReceiveObserverConnector](../../masstransit-abstractions/masstransit/ireceiveobserverconnector), [IReceiveEndpointObserverConnector](../../masstransit-abstractions/masstransit/ireceiveendpointobserverconnector), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [InMemoryReceiveEndpointContext](../masstransit-inmemorytransport/inmemoryreceiveendpointcontext)

## Properties

### **Send**

```csharp
public ISendTopology Send { get; }
```

#### Property Value

[ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

### **MessageFabric**

```csharp
public IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric { get; }
```

#### Property Value

[IMessageFabric\<InMemoryTransportContext, InMemoryTransportMessage\>](../masstransit-transports-fabric/imessagefabric-2)<br/>

### **TransportContext**

```csharp
public InMemoryTransportContext TransportContext { get; }
```

#### Property Value

[InMemoryTransportContext](../masstransit-inmemorytransport/inmemorytransportcontext)<br/>

### **IsBusEndpoint**

```csharp
public bool IsBusEndpoint { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ReceiveObservers**

```csharp
public IReceiveObserver ReceiveObservers { get; }
```

#### Property Value

[IReceiveObserver](../../masstransit-abstractions/masstransit/ireceiveobserver)<br/>

### **TransportObservers**

```csharp
public IReceiveTransportObserver TransportObservers { get; }
```

#### Property Value

[IReceiveTransportObserver](../../masstransit-abstractions/masstransit/ireceivetransportobserver)<br/>

### **EndpointObservers**

```csharp
public IReceiveEndpointObserver EndpointObservers { get; }
```

#### Property Value

[IReceiveEndpointObserver](../../masstransit-abstractions/masstransit/ireceiveendpointobserver)<br/>

### **ConsumerStopTimeout**

```csharp
public Nullable<TimeSpan> ConsumerStopTimeout { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StopTimeout**

```csharp
public Nullable<TimeSpan> StopTimeout { get; }
```

#### Property Value

[Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **DependenciesReady**

```csharp
public Task DependenciesReady { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **DependentsCompleted**

```csharp
public Task DependentsCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

```csharp
public int PrefetchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LogContext**

```csharp
public ILogContext LogContext { get; }
```

#### Property Value

[ILogContext](../masstransit-logging/ilogcontext)<br/>

### **Publish**

```csharp
public IPublishTopology Publish { get; }
```

#### Property Value

[IPublishTopology](../../masstransit-abstractions/masstransit/ipublishtopology)<br/>

### **ReceivePipe**

```csharp
public IReceivePipe ReceivePipe { get; }
```

#### Property Value

[IReceivePipe](../../masstransit-abstractions/masstransit-transports/ireceivepipe)<br/>

### **SendEndpointProvider**

```csharp
public ISendEndpointProvider SendEndpointProvider { get; }
```

#### Property Value

[ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

### **PublishEndpointProvider**

```csharp
public IPublishEndpointProvider PublishEndpointProvider { get; }
```

#### Property Value

[IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

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

### **TransportInMemoryReceiveEndpointContext(IInMemoryHostConfiguration, IInMemoryReceiveEndpointConfiguration)**

```csharp
public TransportInMemoryReceiveEndpointContext(IInMemoryHostConfiguration hostConfiguration, IInMemoryReceiveEndpointConfiguration configuration)
```

#### Parameters

`hostConfiguration` [IInMemoryHostConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryhostconfiguration)<br/>

`configuration` [IInMemoryReceiveEndpointConfiguration](../masstransit-inmemorytransport-configuration/iinmemoryreceiveendpointconfiguration)<br/>

## Methods

### **AddSendAgent(IAgent)**

```csharp
public void AddSendAgent(IAgent agent)
```

#### Parameters

`agent` [IAgent](../../masstransit-abstractions/masstransit/iagent)<br/>

### **AddConsumeAgent(IAgent)**

```csharp
public void AddConsumeAgent(IAgent agent)
```

#### Parameters

`agent` [IAgent](../../masstransit-abstractions/masstransit/iagent)<br/>

### **ConvertException(Exception, String)**

```csharp
public Exception ConvertException(Exception exception, string message)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`message` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **ConfigureTopology()**

```csharp
public void ConfigureTopology()
```

### **CreateSendTransportProvider()**

```csharp
protected ISendTransportProvider CreateSendTransportProvider()
```

#### Returns

[ISendTransportProvider](../../masstransit-abstractions/masstransit-transports/isendtransportprovider)<br/>

### **CreatePublishTransportProvider()**

```csharp
protected IPublishTransportProvider CreatePublishTransportProvider()
```

#### Returns

[IPublishTransportProvider](../../masstransit-abstractions/masstransit-transports/ipublishtransportprovider)<br/>
