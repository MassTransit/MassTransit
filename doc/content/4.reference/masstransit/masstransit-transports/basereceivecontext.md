---

title: BaseReceiveContext

---

# BaseReceiveContext

Namespace: MassTransit.Transports

```csharp
public abstract class BaseReceiveContext : ScopePipeContext, ReceiveContext, PipeContext, IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopePipeContext](../../masstransit-abstractions/masstransit-middleware/scopepipecontext) → [BaseReceiveContext](../masstransit-transports/basereceivecontext)<br/>
Implements [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext), [PipeContext](../../masstransit-abstractions/masstransit/pipecontext), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **CancellationToken**

```csharp
public CancellationToken CancellationToken { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **IsDelivered**

```csharp
public bool IsDelivered { get; private set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFaulted**

```csharp
public bool IsFaulted { get; private set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PublishFaults**

```csharp
public bool PublishFaults { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Body**

```csharp
public abstract MessageBody Body { get; }
```

#### Property Value

[MessageBody](../../masstransit-abstractions/masstransit/messagebody)<br/>

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

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **AddReceiveTask(Task)**

```csharp
public void AddReceiveTask(Task task)
```

#### Parameters

`task` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

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

### **GetSendEndpointProvider()**

```csharp
protected ISendEndpointProvider GetSendEndpointProvider()
```

#### Returns

[ISendEndpointProvider](../../masstransit-abstractions/masstransit/isendendpointprovider)<br/>

### **GetPublishEndpointProvider()**

```csharp
protected IPublishEndpointProvider GetPublishEndpointProvider()
```

#### Returns

[IPublishEndpointProvider](../../masstransit-abstractions/masstransit/ipublishendpointprovider)<br/>

### **GetContentType()**

```csharp
protected ContentType GetContentType()
```

#### Returns

ContentType<br/>

### **Cancel()**

```csharp
public void Cancel()
```

### **ConvertToContentType(String)**

```csharp
protected static ContentType ConvertToContentType(string text)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

ContentType<br/>
