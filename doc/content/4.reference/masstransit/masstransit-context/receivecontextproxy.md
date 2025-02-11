---

title: ReceiveContextProxy

---

# ReceiveContextProxy

Namespace: MassTransit.Context

```csharp
public abstract class ReceiveContextProxy : ReceiveContext, PipeContext
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveContextProxy](../masstransit-context/receivecontextproxy)<br/>
Implements [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext)

## Properties

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

## Methods

### **HasPayloadType(Type)**

```csharp
public bool HasPayloadType(Type contextType)
```

#### Parameters

`contextType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **TryGetPayload\<TPayload\>(TPayload)**

```csharp
public bool TryGetPayload<TPayload>(out TPayload payload)
```

#### Type Parameters

`TPayload`<br/>

#### Parameters

`payload` TPayload<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetOrAddPayload\<TPayload\>(PayloadFactory\<TPayload\>)**

```csharp
public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
```

#### Type Parameters

`TPayload`<br/>

#### Parameters

`payloadFactory` [PayloadFactory\<TPayload\>](../../masstransit-abstractions/masstransit/payloadfactory-1)<br/>

#### Returns

TPayload<br/>

### **NotifyConsumed\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

```csharp
public Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

```csharp
public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **NotifyFaulted(Exception)**

```csharp
public Task NotifyFaulted(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddReceiveTask(Task)**

```csharp
public void AddReceiveTask(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
