---

title: InMemoryReceiveContext

---

# InMemoryReceiveContext

Namespace: MassTransit.InMemoryTransport

```csharp
public sealed class InMemoryReceiveContext : BaseReceiveContext, ReceiveContext, PipeContext, IDisposable, RoutingKeyConsumeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopePipeContext](../../masstransit-abstractions/masstransit-middleware/scopepipecontext) → [BaseReceiveContext](../masstransit-transports/basereceivecontext) → [InMemoryReceiveContext](../masstransit-inmemorytransport/inmemoryreceivecontext)<br/>
Implements [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable), [RoutingKeyConsumeContext](../../masstransit-abstractions/masstransit/routingkeyconsumecontext)

## Properties

### **Body**

```csharp
public MessageBody Body { get; }
```

#### Property Value

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

### **RoutingKey**

```csharp
public string RoutingKey { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **IsDelivered**

```csharp
public bool IsDelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFaulted**

```csharp
public bool IsFaulted { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

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

### **ReceiveCompleted**

```csharp
public Task ReceiveCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Redelivered**

```csharp
public bool Redelivered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TransportHeaders**

```csharp
public Headers TransportHeaders { get; }
```

#### Property Value

[Headers](../../masstransit-abstractions/masstransit/headers)<br/>

### **ElapsedTime**

```csharp
public TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; protected set; }
```

#### Property Value

Uri<br/>

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

## Constructors

### **InMemoryReceiveContext(InMemoryTransportMessage, InMemoryReceiveEndpointContext)**

```csharp
public InMemoryReceiveContext(InMemoryTransportMessage message, InMemoryReceiveEndpointContext receiveEndpointContext)
```

#### Parameters

`message` [InMemoryTransportMessage](../masstransit-inmemorytransport/inmemorytransportmessage)<br/>

`receiveEndpointContext` [InMemoryReceiveEndpointContext](../masstransit-inmemorytransport/inmemoryreceiveendpointcontext)<br/>
