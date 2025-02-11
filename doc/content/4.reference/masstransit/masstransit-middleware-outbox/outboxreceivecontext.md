---

title: OutboxReceiveContext

---

# OutboxReceiveContext

Namespace: MassTransit.Middleware.Outbox

```csharp
public class OutboxReceiveContext : ReceiveContextProxy, ReceiveContext, PipeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveContextProxy](../masstransit-context/receivecontextproxy) → [OutboxReceiveContext](../masstransit-middleware-outbox/outboxreceivecontext)<br/>
Implements [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

### **PublishEndpointProvider**

```csharp
public IPublishEndpointProvider PublishEndpointProvider { get; }
```

#### Property Value

[IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

### **SendEndpointProvider**

```csharp
public ISendEndpointProvider SendEndpointProvider { get; }
```

#### Property Value

[ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Body**

```csharp
public MessageBody Body { get; }
```

#### Property Value

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

### **ElapsedTime**

```csharp
public TimeSpan ElapsedTime { get; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **InputAddress**

```csharp
public Uri InputAddress { get; }
```

#### Property Value

Uri<br/>

### **ContentType**

```csharp
public ContentType ContentType { get; }
```

#### Property Value

ContentType<br/>

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

### **ReceiveCompleted**

```csharp
public Task ReceiveCompleted { get; }
```

#### Property Value

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

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

## Constructors

### **OutboxReceiveContext(OutboxSendContext, ReceiveContext)**

```csharp
public OutboxReceiveContext(OutboxSendContext outboxContext, ReceiveContext context)
```

#### Parameters

`outboxContext` [OutboxSendContext](../masstransit-middleware/outboxsendcontext)<br/>

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>
