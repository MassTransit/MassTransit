---

title: ConsumerAgent<TKey>

---

# ConsumerAgent\<TKey\>

Namespace: MassTransit.Transports

```csharp
public abstract class ConsumerAgent<TKey> : Agent, IAgent, DeliveryMetrics
```

#### Type Parameters

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Agent](../../masstransit-abstractions/masstransit-middleware/agent) → [ConsumerAgent\<TKey\>](../masstransit-transports/consumeragent-1)<br/>
Implements [IAgent](../../masstransit-abstractions/masstransit/iagent), [DeliveryMetrics](../masstransit-transports/deliverymetrics)

## Properties

### **DeliveryCount**

```csharp
public long DeliveryCount { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ConcurrentDeliveryCount**

```csharp
public int ConcurrentDeliveryCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

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

## Methods

### **TrySetManualConsumeTask()**

```csharp
protected void TrySetManualConsumeTask()
```

### **TrySetConsumeTask(Task)**

```csharp
protected void TrySetConsumeTask(Task consumeTask)
```

#### Parameters

`consumeTask` [Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **StopAgent(StopContext)**

```csharp
protected Task StopAgent(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **TrySetConsumeCompleted()**

```csharp
protected void TrySetConsumeCompleted()
```

### **TrySetConsumeCanceled(CancellationToken)**

```csharp
protected void TrySetConsumeCanceled(CancellationToken cancellationToken)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **TrySetConsumeException(Exception)**

```csharp
protected void TrySetConsumeException(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **ActiveAndActualAgentsCompleted(StopContext)**

```csharp
protected Task ActiveAndActualAgentsCompleted(StopContext context)
```

#### Parameters

`context` [StopContext](../../masstransit-abstractions/masstransit/stopcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **IsTrackable(TKey)**

```csharp
protected bool IsTrackable(TKey key)
```

#### Parameters

`key` TKey<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Dispatch\<TContext\>(TKey, TContext, ReceiveLockContext)**

```csharp
protected Task Dispatch<TContext>(TKey key, TContext context, ReceiveLockContext receiveLockContext)
```

#### Type Parameters

`TContext`<br/>

#### Parameters

`key` TKey<br/>

`context` TContext<br/>

`receiveLockContext` [ReceiveLockContext](../masstransit-transports/receivelockcontext)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
